using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Bolt.EntityEventListener<IPlayerState>
{
    private Rigidbody rb;
    public float speed;
    private Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
    private Player localPlayer;
    private InteractiveObject objectInFocus;

    public WizardActive activeItem;
    public WizardWeapon wizardWeapon;

    public override void Attached()
    {
        rb = GetComponent<Rigidbody>();
        state.SetTransforms(state.transform, transform);
    }

    public void Update() {
        if (!entity.isOwner) return;

        if (localPlayer == null) {
            CheckForPlayer();
            return;
        }
        DoMovement();
        DoLook();
        CheckInteract();
        if (localPlayer.GetButtonDown("Interact")) DoInteract();
        if (localPlayer.GetButtonDown("Fire")) wizardWeapon.FireDown();
        if (localPlayer.GetButton("Fire")) wizardWeapon.FireHold();
        if (localPlayer.GetButtonUp("Fire")) wizardWeapon.FireRelease();
        if (activeItem != null && localPlayer.GetButtonDown("UseActive")) activeItem.Activate();
    }

    private void CheckForPlayer() {
        foreach (Player p in ReInput.players.Players) {
            if (p.GetAnyButton() && !p.isPlaying) {
                AssignPlayer(p.id);
            }
        }
    }

    private void DoMovement() {
        float moveHorizontal = localPlayer.GetAxis("Horizontal");
        float moveVertical = localPlayer.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(transform.position + movement * speed * BoltNetwork.FrameDeltaTime);
    }

    private void DoLook() {
        if (localPlayer.id == 0) {
            // Player is using kbm.
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (aimPlane.Raycast(ray, out distance)) {
                Vector3 target = ray.GetPoint(distance);
                target.y = gameObject.transform.position.y;
                gameObject.transform.LookAt(target);
            }
        } else {
            // Player is using gamepad.
            float lookHorizontal = localPlayer.GetAxis("LookHorizontal");
            float lookVertical = localPlayer.GetAxis("LookVertical");
            Vector3 lookDir = new Vector3(lookHorizontal, 0f, lookVertical);
            if (lookDir.magnitude > 0f) {
                transform.LookAt(transform.position + lookDir);
            }
        }
    }

    private void CheckInteract() {
        Vector3 boxSize = new Vector3(.35f, 1f, .4f);
        Collider[] overlap = Physics.OverlapBox(transform.position + transform.forward * .26f + transform.up * .51f, boxSize / 2, transform.rotation);
        InteractiveObject closest = null;

        // Check for interactive.
        foreach (Collider c in overlap) {
            if (c.tag == "Interactive") {
                InteractiveObject io = c.GetComponent<InteractiveObject>();
                if (io != null) {
                    if (closest == null || Vector3.Distance(transform.position, c.transform.position) < Vector3.Distance(transform.position, io.transform.position))
                        closest = io;
                } 
            }
        }

        // Update Highlights.
        if (closest != null) {
            if (objectInFocus != null && objectInFocus != closest) {
                objectInFocus.RemoveHighlight();
                objectInFocus = closest;
                objectInFocus.AddHighlight();
            } else if (objectInFocus == null) {
                objectInFocus = closest;
                closest.AddHighlight();
            }
        } else if (objectInFocus != null) {
            objectInFocus.RemoveHighlight();
            objectInFocus = null;
        }
    }

    private void DoInteract() {
        if (objectInFocus != null)
            objectInFocus.DoInteract(entity);
    }

    public void GetPickup(ItemPickup pickup) {
        PlayerGotItem evnt = PlayerGotItem.Create(entity);
        evnt.Pickup = pickup.entity;
        evnt.Send();
    }

    public override void OnEvent(PlayerGotItem evnt) {
        ItemPickup pickup = evnt.Pickup.GetComponent<ItemPickup>();
        WizardFightItem item = ItemManager.Instance.items[pickup.Id];
        GameObject pickupObj = Instantiate(item.HeldModel, transform);
        if (item.Type == WizardFightItem.ItemType.Active) {
            ActiveItemChanged((WizardActive)item.HeldScript);
        } else if (item.Type == WizardFightItem.ItemType.Weapon) {
            WizardWeaponChanged((WizardWeapon)item.HeldScript);
        }
    }

    private void ActiveItemChanged(WizardActive active) {
        DropActive();
        activeItem = active;
    }

    private void WizardWeaponChanged(WizardWeapon wep) {
        DropWeapon();
        wizardWeapon = wep;
    }

    private void DropActive() {
        if (activeItem != null)
            Destroy(activeItem.gameObject);
        SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
        evnt.ItemId = activeItem.Id;
        evnt.Position = transform.position;
        evnt.Force = transform.forward;
        evnt.Send();
    }

    private void DropWeapon() {
        if (wizardWeapon != null)
            Destroy(wizardWeapon.gameObject);
        SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
        evnt.ItemId = wizardWeapon.Id;
        evnt.Position = transform.position;
        evnt.Force = transform.forward;
        evnt.Send();
    }

    public void AssignPlayer(int id) {
        if (entity.isOwner) {
            Debug.LogFormat("Assigning player id {0}", id);
            localPlayer = ReInput.players.GetPlayer(id);
            localPlayer.isPlaying = true;
        } else {
            Debug.Log("Please only assign local player as networked owner.");
        }
    }
}
