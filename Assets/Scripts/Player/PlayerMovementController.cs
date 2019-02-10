using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Bolt.EntityEventListener<IPlayerState>
{
    private Rigidbody rb;
    public float BaseSpeed;
    private Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
    private Player localPlayer;
    private InteractiveObject objectInFocus;

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
        if (localPlayer.GetButtonDown("Fire")) state.FireDown();
        if (localPlayer.GetButton("Fire")) state.FireHold();
        if (localPlayer.GetButtonUp("Fire")) state.FireRelease();
        if (localPlayer.GetButtonDown("UseActive")) state.ActiveDown();
        if (localPlayer.GetButton("UseActive")) state.ActiveHold();
        if (localPlayer.GetButtonUp("UseActive")) state.ActiveRelease();
    }

    private void CheckForPlayer() {
        foreach (Player p in ReInput.players.Players) {
            if (p.GetAnyButton()) {
                Debug.LogFormat("{0}: {1}", p.id, p.GetAnyButton());
            }
            
            if (p.GetAnyButton() && !p.isPlaying) {
                AssignPlayer(p.id);
            }
        }
    }

    private void DoMovement() {
        float moveHorizontal = localPlayer.GetAxis("Horizontal");
        float moveVertical = localPlayer.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(transform.position + movement * BaseSpeed * Time.deltaTime * state.Speed);
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
                objectInFocus.FocusLost();
                objectInFocus = closest;
                objectInFocus.FocusGained();
            } else if (objectInFocus == null) {
                objectInFocus = closest;
                closest.FocusGained();
            }
        } else if (objectInFocus != null) {
            objectInFocus.FocusLost();
            objectInFocus = null;
        }
    }

    private void DoInteract() {
        if (objectInFocus != null)
            objectInFocus.DoInteract(entity);
    }

    public void GetPickup(ItemPickup pickup) {
        PlayerGotItem evnt = PlayerGotItem.Create(entity);
        evnt.PickupId = pickup.Id;
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
