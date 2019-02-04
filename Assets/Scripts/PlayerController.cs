using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Bolt.EntityBehaviour<IPlayerState>
{

    public List<WizardWeapon> Weapons = new List<WizardWeapon>();
    public List<WizardActive> ActiveItems = new List<WizardActive>();

    private Rigidbody rb;
    public float speed;
    private Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
    private Player localPlayer;

    private WizardActive activeItem;
    private WizardWeapon wizardWeapon;

    public override void Attached()
    {
        rb = GetComponent<Rigidbody>();
        state.SetTransforms(state.transform, transform);
        state.ActiveItem = -1;
        state.AddCallback("WeaponId", WizardWeaponChanged);
        state.AddCallback("ActiveItem", ActiveItemChanged);
        // We have to call this as the state defaults to 0;
        WizardWeaponChanged();
    }

    public void Update() {
        if (!entity.isOwner) return;

        if (localPlayer == null) {
            CheckForPlayer();
            return;
        }
        DoMovement();
        DoLook();
        if (localPlayer.GetButtonDown("Fire")) wizardWeapon.Fire();
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

    private void ActiveItemChanged() {
        if (activeItem != null)
            activeItem.gameObject.SetActive(false);
        if (state.ActiveItem > -1) {
            activeItem = ActiveItems[state.ActiveItem];
            activeItem.gameObject.SetActive(true);
            activeItem.OnEquip();
        } else {
            activeItem = null;
        }
    }

    private void WizardWeaponChanged() {
        Debug.Log("Set weapon to " + state.WeaponId);
        if (wizardWeapon != null)
            wizardWeapon.gameObject.SetActive(false);
        wizardWeapon = Weapons[state.WeaponId];
        wizardWeapon.gameObject.SetActive(true);
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
