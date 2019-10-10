using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnActiveChange))]
    private ItemDefinition activeDef;
    public ActiveItem activeItem;
    [SyncVar(hook = nameof(OnWeaponChange))]
    private ItemDefinition weaponDef;
    public Weapon wizardWeapon;
    public Transform launchPos;
    private Transform playerHand;
    private PlayerUI ui;

    private List<HeldPassive> passiveItems = new List<HeldPassive>();

    [SyncVar]
    private int storedUses = 0; // TEMP FIX - Can't pass data into callbacks and need this done right now. 

    public void Awake() {
        ui = GetComponent<PlayerUI>();

        playerHand = GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        if (isLocalPlayer)
            StartCoroutine("WaitForItemManager");
    }

    IEnumerator WaitForItemManager() {
        while (ItemManager.Instance == null) {
            yield return new WaitForSeconds(0.1f);
        }

        // Give basic wand
        weaponDef = ItemManager.Instance.items[0];
    }

    private void PlayerDied() {
        CmdDropActive();
        CmdDropWeapon();
    }

    private void OnWeaponChange() {
        if (weaponDef == null) {
            Destroy(wizardWeapon.gameObject);
            wizardWeapon = null;
            if (isLocalPlayer)
                ui.SetWeapon(-1);
        } else {
            Vector3 handOffset = new Vector3(-2.65f, -1.7f, .63f);
            Quaternion handRotation = Quaternion.Euler(-26.35f, -13.78f, 148.35f);
            GameObject newWep = Instantiate(weaponDef.HeldModel, playerHand);
            newWep.transform.localPosition = handOffset;
            newWep.transform.localRotation = handRotation;
            if (isLocalPlayer) {
                wizardWeapon = newWep.GetComponent<Weapon>();
                wizardWeapon.Owner = this;
                WeaponUses uses = wizardWeapon.GetComponent<WeaponUses>();
                if (uses) uses.AmountUsed = storedUses;
                ui.SetWeapon(wizardWeapon.Id);
                wizardWeapon.OnEquip();
            }
        }
    }

    private void OnActiveChange() {
        if (activeDef == null) {
            Destroy(activeItem.gameObject);
            activeItem = null;
            if (isLocalPlayer)
                ui.SetActiveItem(-1);
        } else {
            GameObject newActive = Instantiate(activeDef.HeldModel, transform);
            if (isLocalPlayer) {
                ui.SetActiveItem(activeDef.ItemId);
                activeItem = newActive.GetComponent<ActiveItem>();
                activeItem.Owner = this;
                ActiveUses uses = activeItem.GetComponent<ActiveUses>();
                if (uses) uses.AmountUsed = storedUses;
                activeItem.OnEquip();
            }
        }
    }

    public void FireDown() {
        if (wizardWeapon != null)
            wizardWeapon.FireDown();
    }

    public void FireHeld() {
        if (wizardWeapon != null)
            wizardWeapon.FireHold();
    }

    public void FireRelease() {
        if (wizardWeapon != null)
            wizardWeapon.FireRelease();
    }

    public void ActiveDown() {
        if (activeItem != null)
            activeItem.ActiveDown();
    }

    public void ActiveHold() {
        if (activeItem != null)
            activeItem.ActivateHold();
    }

    public void ActiveRelease() {
        if (activeItem != null)
            activeItem.ActivateRelease();
    }

    [Command]
    public void CmdGiveItem(DroppedItem item) {
        ItemDefinition def = ItemManager.Instance.items[item.Id];
        if (def.Type == ItemDefinition.ItemType.Weapon) {
            CmdSetWeapon(item);
        } else if (def.Type == ItemDefinition.ItemType.Active) {
            CmdSetActive(item);
        } else if (def.Type == ItemDefinition.ItemType.Passive) {
            CmdGivePassive(item);
        }
    }

    [Command]
    public void CmdSetWeapon(DroppedItem item) {
        CmdDropWeapon();
        weaponDef = ItemManager.Instance.items[item.Id];
        storedUses = item.Used;
    }

    [Command]
    public void CmdSetActive(DroppedItem item) {
        CmdDropActive();
        activeDef = ItemManager.Instance.items[item.Id];
        storedUses = item.Used;
    }

    [Command]
    public void CmdGivePassive(DroppedItem item) {
        GameObject newPassive = Instantiate(ItemManager.Instance.items[item.Id].HeldModel, transform);
        HeldPassive passive = newPassive.GetComponent<HeldPassive>();
        passive.Id = item.Id;
        passive.Owner = this;
        passiveItems.Add(passive);
        passive.OnEquip();
        NetworkServer.Spawn(newPassive);
    }

    [Command]
    private void CmdDropActive() {
        if (activeItem != null) {
            activeItem.OnDequip();
            Vector3 dropForce = transform.position + transform.forward * 1.3f + transform.up * .5f;
            ActiveUses uses = activeItem.GetComponent<ActiveUses>();
            int used = 0;
            if (uses != null) {
                used = uses.AmountUsed;
            }
            ItemManager.Instance.CmdSpawn(transform.position, dropForce, activeItem.Id, "", used);
            activeDef = null;
        }
    }

    [Command]
    public void CmdDestroyActive() {
        activeDef = null;
    }

    [Command]
    private void CmdDropWeapon() {
        if (wizardWeapon != null) {
            wizardWeapon.OnDequip();
            Vector3 dropForce = transform.position + transform.forward * 1.3f + transform.up * .5f;
            ActiveUses uses = activeItem.GetComponent<ActiveUses>();
            int used = 0;
            if (uses != null) {
                used = uses.AmountUsed;
            }
            ItemManager.Instance.CmdSpawn(transform.position, dropForce, activeItem.Id, gameObject.tag, used);
            weaponDef = null;
        }
    }

    [Command]
    public void CmdDestroyWeapon() {
        weaponDef = null;
    }

    private void DetachAndHideItem(GameObject obj) {
        // Used to cause bugs if the object was removed, seems ok now. Leaving briefly in case bugs resurface.
        //obj.transform.parent = null; 
        //obj.transform.localScale = new Vector3(0, 0, 0);
        //obj.transform.position = new Vector3(999999, 999999, 999999);

        Destroy(obj);
    }
}
