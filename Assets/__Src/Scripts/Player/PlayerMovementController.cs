using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Bolt.EntityEventListener<IPlayerState>
{
    public float BaseSpeed;
    public float BaseAccel;
    public float BaseFriction;
    public Transform RenderTransform;
    private Rigidbody rb;
    private Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
    private Player localPlayer;
    private InteractiveObject objectInFocus;
    private PlayerUI playerUI;
    private Animator anim;

    public override void Attached()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        state.SetTransforms(state.transform, transform, RenderTransform);
        state.SetAnimator(anim);
        anim.applyRootMotion = entity.isOwner;
        if (entity.isOwner)
            playerUI = GetComponent<PlayerUI>();
    }

    public void Update() {
        // Since we're using Rewired we cannot use Bolt's SimulateController as Rewired won't be able to get input.
        // Hence we have to do a check here. localPlayer == null will prevent the server from throwing exceptions when it gets
        // upset that it can't control client's players.
        if (!entity.isOwner || localPlayer == null) return;
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

        UpdateMovementFriction();
        UpdateMovementInputAcceleration(movement);

        state.ForwardMovement = Vector3.Dot(rb.velocity, transform.forward) / (state.Speed * BaseSpeed);
        state.RightMovement = Vector3.Dot(rb.velocity, transform.right) / (state.Speed * BaseSpeed);
    }

    private void UpdateMovementFriction()
    {
        float speed = rb.velocity.magnitude;
        if (speed != 0) 
        {
            float drop = speed * BaseFriction * Time.deltaTime;
            rb.velocity *= Mathf.Max(speed - drop, 0) / speed; 
        }
    }

    private void UpdateMovementInputAcceleration(Vector3 movement)
    {
        Vector3 accelDir = movement.normalized;
        float accelAmount = Mathf.Min(movement.magnitude, 1) * state.Speed * BaseSpeed * BaseAccel;
        float maxVelocity = state.Speed * BaseSpeed;

        float projVel = Vector3.Dot(rb.velocity, accelDir);
        float accelVel = accelAmount * Time.deltaTime;

        if (projVel + accelVel > maxVelocity) accelVel = maxVelocity - projVel;

        rb.velocity = rb.velocity + accelDir * accelVel;
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
        Vector3 boxSize = new Vector3(1f, 1f, 1.2f);
        Collider[] overlap = Physics.OverlapBox(transform.position + transform.forward * .7f + transform.up * .51f, boxSize / 2, transform.rotation);
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

    public void GetPickup(DroppedItemPickup pickup) {
        PlayerGotItem evnt = PlayerGotItem.Create(entity);
        evnt.PickupId = pickup.Id;
        evnt.Send();
    }

    public void AssignPlayer(int id) {
        if (entity.isControllerOrOwner) {
            Debug.LogFormat("Assigning player id {0}", id);
            localPlayer = ReInput.players.GetPlayer(id);
            localPlayer.isPlaying = true;
        } else {
            Debug.Log("Please only assign local player as networked owner.");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isOwner) return;
        if (other.tag == "Room") {
            SplitscreenManager.instance.playerCameras[playerUI.ScreenNumber - 1].AddRoomToCamera(other.transform.Find("Focus"));
        }
    }

    public override void OnEvent(TeleportPlayer evnt) {
        if (entity.isOwner)
            transform.position = evnt.position;
    }
}
