using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : Bolt.EntityEventListener<IPlayerState>
{
    public ActiveItem activeItem;
    public Weapon wizardWeapon;

    public override void Attached() {
        state.WeaponId = 0;
        state.ActiveId = -1;
        state.AddCallback("WeaponId", WeaponIdChanged);
        state.AddCallback("ActiveId", ActiveIdChanged);
        state.OnFireDown += FireDownTrigger;
        state.OnFireHold += FireHeldTrigger;
        state.OnFireRelease += FireReleaseTrigger;
        state.OnActiveDown += ActiveDownTrigger;
        state.OnActiveHold += ActiveHoldTrigger;
        state.OnActiveRelease += ActiveReleaseTrigger;
        WeaponIdChanged();
    }

    private void WeaponIdChanged() {
        DropWeapon();
        GameObject newWep = Instantiate(ItemManager.Instance.items[state.WeaponId].HeldModel, transform);
        newWep.GetComponent<HeldItem>().Id = state.WeaponId;
        wizardWeapon = newWep.GetComponent<Weapon>();
        wizardWeapon.Owner = this;
        wizardWeapon.OnEquip();
    }

    private void ActiveIdChanged() {
        DropActive();
        if (state.ActiveId >= 0) {
            GameObject newActive = Instantiate(ItemManager.Instance.items[state.ActiveId].HeldModel, transform);
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
            if (entity.isOwner) {
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                evnt.ItemId = activeItem.Id;
                evnt.Position = transform.position;
                evnt.Force = transform.forward;
                evnt.Send();
            }
        }
    }

    private void DropWeapon() {
        if (wizardWeapon != null) {
            Destroy(wizardWeapon.gameObject);
            if (entity.isOwner) {
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                evnt.ItemId = wizardWeapon.Id;
                evnt.Position = transform.position + transform.forward * .4f + transform.up * .5f;
                evnt.Force = transform.forward * 50f;
                evnt.Send();
            }
        }
    }
}
