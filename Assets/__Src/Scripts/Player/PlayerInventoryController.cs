﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : Bolt.EntityEventListener<IPlayerState>
{
    public ActiveItem activeItem;
    public Weapon wizardWeapon;
    private Transform playerHand;
    private PlayerUI ui;

    private List<HeldPassive> passiveItems = new List<HeldPassive>();

    private int storedUsesUsed = 0; // TEMP FIX - Can't pass data into callbacks and need this done right now. 

    public override void Attached() {
        ui = GetComponent<PlayerUI>();

        if (entity.isOwner) {
            state.WeaponId = -1;
            state.ActiveId = -1;
        }
        
        state.AddCallback("WeaponId", WeaponIdChanged);
        state.AddCallback("ActiveId", ActiveIdChanged);
        state.AddCallback("Dead", PlayerDied);

        state.OnAddPassive += AddPassive;
        state.OnFireDown += FireDownTrigger;
        state.OnFireHold += FireHeldTrigger;
        state.OnFireRelease += FireReleaseTrigger;
        state.OnActiveDown += ActiveDownTrigger;
        state.OnActiveHold += ActiveHoldTrigger;
        state.OnActiveRelease += ActiveReleaseTrigger;
        state.OnDestroyActive += DestroyActive;
        state.OnDestroyWeapon += DestroyWeapon;

        playerHand = GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        StartCoroutine("WaitForItemManager");
    }

    private void GrantStarterItems()
    {
        if (entity.isOwner) {
            state.WeaponId = 0;
        }
    }

    IEnumerator WaitForItemManager() {
        while (ItemManager.Instance == null) {
            yield return new WaitForSeconds(0.1f);
        }
        // Update initial UI.
        WeaponIdChanged();
        ActiveIdChanged();

        GrantStarterItems();
    }

    private void PlayerDied() {
        DropActive();
        DropWeapon();
    }

    private void AddPassive() {
        GameObject newPassive = Instantiate(ItemManager.Instance.items[state.NewPassiveId].HeldModel, transform);
        HeldPassive passive = newPassive.GetComponent<HeldPassive>();
        passive.Id = state.NewPassiveId;
        passive.Owner = this;
        passiveItems.Add(passive);
        passive.OnEquip();
    }

    private void WeaponIdChanged() {
        DropWeapon();
        if (entity.hasControl) ui.SetWeapon(state.WeaponId);

        if (state.WeaponId >= 0) {
            ItemDefinition item = ItemManager.Instance.items[state.WeaponId];

            Vector3 handOffset = new Vector3(-2.65f, -1.7f, .63f);
            Quaternion handRotation = Quaternion.Euler(-26.35f, -13.78f, 148.35f);
            GameObject newWep = Instantiate(item.HeldModel, playerHand);
            newWep.transform.localPosition = handOffset;
            newWep.transform.localRotation = handRotation;
            newWep.GetComponent<HeldItem>().Id = state.WeaponId;
            wizardWeapon = newWep.GetComponent<Weapon>();
            wizardWeapon.Owner = this;

            WeaponUses uses = wizardWeapon.GetComponent<WeaponUses>();
            if (uses) uses.AmountUsed = storedUsesUsed;

            wizardWeapon.OnEquip();
        }
    }

    private void ActiveIdChanged() {
        DropActive();
        if (entity.hasControl) ui.SetActiveItem(state.ActiveId);

        if (state.ActiveId >= 0)
        {
            ItemDefinition item = ItemManager.Instance.items[state.ActiveId];

            GameObject newActive = Instantiate(item.HeldModel, transform);
            newActive.GetComponent<HeldItem>().Id = state.ActiveId;
            activeItem = newActive.GetComponent<ActiveItem>();
            activeItem.Owner = this;

            ActiveUses uses = activeItem.GetComponent<ActiveUses>();
            if (uses) uses.AmountUsed = storedUsesUsed;

            activeItem.OnEquip();
        }
    }

    private void FireDownTrigger() {
        if (wizardWeapon != null)
            wizardWeapon.FireDown();
    }

    private void FireHeldTrigger() {
        if (wizardWeapon != null)
            wizardWeapon.FireHold();
    }

    private void FireReleaseTrigger() {
        if (wizardWeapon != null)
            wizardWeapon.FireRelease();
    }

    private void ActiveDownTrigger() {
        if (activeItem != null)
            activeItem.ActiveDown();
    }

    private void ActiveHoldTrigger() {
        if (activeItem != null)
            activeItem.ActivateHold();
    }

    private void ActiveReleaseTrigger() {
        if (activeItem != null)
            activeItem.ActivateRelease();
    }

    public override void OnEvent(PlayerGotItem pickup) {
        ItemDefinition item = ItemManager.Instance.items[pickup.PickupId];

        storedUsesUsed = pickup.UsesUsed; // Hack to pass data into IdChanged callbacks. 

        if (item.Type == ItemDefinition.ItemType.Weapon) {
            state.WeaponId = pickup.PickupId;
        } else if (item.Type == ItemDefinition.ItemType.Active) {
            state.ActiveId = pickup.PickupId;
        } else if (item.Type == ItemDefinition.ItemType.Passive) {
            state.NewPassiveId = pickup.PickupId;
            AddPassive();
        }


        if (wizardWeapon != null && pickup.PickupId == wizardWeapon.Id) {
            WeaponIdChanged();
        } else if (activeItem != null && pickup.PickupId == activeItem.Id) {
            ActiveIdChanged();
        }
    }

    private void DropActive() {
        if (activeItem != null) {
            activeItem.OnDequip();
            if (entity.isControllerOrOwner) {
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                evnt.ItemId = activeItem.Id;
                evnt.Position = transform.position;
                evnt.Force = transform.forward;
                evnt.SpawnerTag = gameObject.tag;

                ActiveUses uses = activeItem.GetComponent<ActiveUses>();
                if (uses != null) {
                    evnt.UsesUsed = uses.AmountUsed;
                }

                evnt.Send();
            }
            DetachAndHideItem(activeItem.gameObject);
        }
    }

    private void DestroyActive() {
        if (activeItem != null) {
            activeItem.OnDequip();
            DetachAndHideItem(activeItem.gameObject);
            state.ActiveId = -1;
            activeItem = null;
        }
    }

    private void DropWeapon() {
        if (wizardWeapon != null) {
            wizardWeapon.OnDequip();
            if (entity.isControllerOrOwner) {
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                evnt.ItemId = wizardWeapon.Id;
                evnt.Position = transform.position + transform.forward * .4f + transform.up * .5f;
                evnt.Force = transform.forward * 50f;
                evnt.SpawnerTag = gameObject.tag;

                WeaponUses uses = wizardWeapon.GetComponent<WeaponUses>();
                if (uses != null) {
                    evnt.UsesUsed = uses.AmountUsed;
                }

                evnt.Send();
            }
            DetachAndHideItem(wizardWeapon.gameObject);
        }
    }

    private void DestroyWeapon() {
        if (wizardWeapon != null) {
            wizardWeapon.OnDequip();
            DetachAndHideItem(wizardWeapon.gameObject);
            state.WeaponId = -1;
            wizardWeapon = null;
        }
    }

    private void DetachAndHideItem(GameObject obj) {
        // Detach and hide without losing references (for instance airborne projectiles may need it).
        // Need to revisit this later.
        obj.transform.parent = null; 
        obj.transform.localScale = new Vector3(0, 0, 0);
    }
}
