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
        GameObject pickupObj = Instantiate(pickup.pickupPrefab, transform);
        if (pickup.pickupType == ItemPickup.PickupType.Active) {
            ActiveItemChanged(pickupObj.GetComponent<WizardActive>());
        } else if (pickup.pickupType == ItemPickup.PickupType.Weapon) {
            WizardWeaponChanged(pickupObj.GetComponent<WizardWeapon>());
        }
    }

    private void ActiveItemChanged(WizardActive active) {
        if (activeItem != null)
            Destroy(activeItem.gameObject);
        activeItem = active;
        activeItem.OnEquip();
    }

    private void WizardWeaponChanged(WizardWeapon wep) {
        Debug.Log("Set weapon to " + state.WeaponId);
        if (wizardWeapon != null)
            Destroy(wizardWeapon.gameObject);
        wizardWeapon = wep;
        wizardWeapon.OnEquip();
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
