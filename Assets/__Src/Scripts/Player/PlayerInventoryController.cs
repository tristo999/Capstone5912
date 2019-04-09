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

    private static readonly int EMPTY_ITEM_CAN_DROP = -1;
    private static readonly int EMPTY_ITEM_NO_DROP = -2; // Using this to fix item uses running out quicky.

    public override void Attached() {
        ui = GetComponent<PlayerUI>();

        if (entity.isOwner) {
            state.WeaponId = EMPTY_ITEM_CAN_DROP;
            state.ActiveId = EMPTY_ITEM_CAN_DROP;
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
        if (state.WeaponId != EMPTY_ITEM_NO_DROP) {
            DropWeapon();
        }
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
            activeItem.OnEquip();
        }
    }

    private void FireDownTrigger() {
            wizardWeapon.FireDown();
    }

    private void FireHeldTrigger() {
            wizardWeapon.FireHold();
    }

    private void FireReleaseTrigger() {
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
        if (item.Type == ItemDefinition.ItemType.Weapon)
            state.WeaponId = pickup.PickupId;
        else if (item.Type == ItemDefinition.ItemType.Active)
            state.ActiveId = pickup.PickupId;
        if (pickup.PickupId == wizardWeapon.Id) {
            WeaponIdChanged();
        } else if (activeItem != null && pickup.PickupId == activeItem.Id) {
            ActiveIdChanged();
        }
    }

    private void DropActive() {
        if (activeItem != null) {
            Destroy(activeItem.gameObject);
            if (entity.isControllerOrOwner) {
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                evnt.ItemId = activeItem.Id;
                evnt.Position = transform.position;
                evnt.Force = transform.forward;
                evnt.Send();
            }
        }
    }

    private void DestroyActive() {
        if (activeItem != null) {
            Destroy(activeItem.gameObject);
            state.ActiveId = EMPTY_ITEM_NO_DROP;
        }
    }

    private void DropWeapon() {
        if (wizardWeapon != null) {
            Destroy(wizardWeapon.gameObject);
            if (entity.isControllerOrOwner) {
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                evnt.ItemId = wizardWeapon.Id;
                evnt.Position = transform.position + transform.forward * .4f + transform.up * .5f;
                evnt.Force = transform.forward * 50f;
                evnt.Send();
            }
        }
    }

    private void DestroyWeapon() {
        if (wizardWeapon != null) {
            Destroy(wizardWeapon.gameObject);
            state.WeaponId = EMPTY_ITEM_NO_DROP;
        }
    }
}
