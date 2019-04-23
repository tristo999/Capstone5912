using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Bolt.EntityEventListener<IPlayerState>
{
    public float BaseSpeed;
    public float BaseAccel;
    public float BaseFriction;
    public bool InputDisabled;
    public Transform RenderTransform;
    private Rigidbody rb;
    private Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
    public Player localPlayer;

    private InteractiveObject objectInFocusField;
    private InteractiveObject ObjectInFocus
    {
        get
        {
            return objectInFocusField;
        }
        set
        {
            objectInFocusField = value;

            // Update item description in UI.
            if (entity.hasControl)
            {
                if (value is DroppedItem)
                {
                    ui.SetItemFullDescription(((DroppedItem)value).Id);
                }
                else
                {
                    ui.SetItemFullDescription(-1);
                }
            }
        }
    }

    private PlayerUI ui;
    private Animator anim;
    private bool thirdPerson;

    public override void Attached()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        state.SetTransforms(state.transform, transform, RenderTransform);
        state.SetAnimator(anim);
        state.AddCallback("Dead", PlayerDied);
        if (entity.isOwner) ui = GetComponent<PlayerUI>();
    }

    public void Update() {
        // Since we're using Rewired we cannot use Bolt's SimulateController as Rewired won't be able to get input.
        // Hence we have to do a check here. localPlayer == null will prevent the server from throwing exceptions when it gets
        // upset that it can't control client's players.
        if (!entity.isOwner || localPlayer == null) return;
        if (InputDisabled) return;
        DoLook();
        CheckInteract();
        if (localPlayer.GetButtonDown("Interact")) DoInteract();
        if (localPlayer.GetButtonDown("Fire")) state.FireDown();
        if (localPlayer.GetButton("Fire")) state.FireHold();
        if (localPlayer.GetButtonUp("Fire")) state.FireRelease();
        if (localPlayer.GetButtonDown("UseActive")) state.ActiveDown();
        if (localPlayer.GetButton("UseActive")) state.ActiveHold();
        if (localPlayer.GetButtonUp("UseActive")) state.ActiveRelease();
        if (localPlayer.GetButtonDown("ChangeView")) ToggleView();
        if (localPlayer.GetButtonDown("Pause")) PauseMenu.Instance.TogglePauseMenu();
    }

    public void FixedUpdate() {
        if (!entity.isOwner || localPlayer == null) return;

        // Custom gravity for the players. They are far too floaty with default gravity.
        rb.AddForce(Physics.gravity * rb.mass * 2.5f); // was 3.5f

        DoMovement();
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

    private void PlayerDied() {
        InputDisabled = true;
        state.Health = 0;
    }

    private void DoMovement() {
        Vector3 movement = Vector3.zero;
        if (!InputDisabled) {
            float moveHorizontal = localPlayer.GetAxis("Horizontal");
            float moveVertical = localPlayer.GetAxis("Vertical");
            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            if (thirdPerson) {
                movement = SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].camera.transform.TransformDirection(movement);
                movement.y = 0;
            }
        }        

        if (CalculateIsOnGround()) {
            UpdateMovementGround(movement);
        } else {
            UpdateMovementAir(movement);
        }

        state.ForwardMovement = Vector3.Dot(rb.velocity, transform.forward) / (state.Speed * BaseSpeed);
        state.RightMovement = Vector3.Dot(rb.velocity, transform.right) / (state.Speed * BaseSpeed);
    }

    private void UpdateMovementGround(Vector3 movement) {
        rb.velocity = CalculateVelocityFromFriction();
        rb.velocity = CalculateVelocityFromInputAccel(movement, 1, 1);
    }

    private void UpdateMovementAir(Vector3 movement) {
        rb.velocity = CalculateVelocityFromInputAccel(movement, 1/4.5f, 1f);
    }

    private Vector3 CalculateVelocityFromFriction() {
        float speed = rb.velocity.magnitude;
        if (speed != 0) {
            float drop = speed * BaseFriction * Time.fixedDeltaTime;
            float frictionFactor = Mathf.Max(speed - drop, 0) / speed;
            return new Vector3(rb.velocity.x * frictionFactor, rb.velocity.y, rb.velocity.z * frictionFactor);
        } else {
            return rb.velocity;
        }
    }

    private Vector3 CalculateVelocityFromInputAccel(Vector3 movement, float accelAmountMultiplier, float maxVelocityMultiplier) {
        Vector3 accelDir = movement.normalized;
        float accelAmount = Mathf.Min(movement.magnitude, 1) * state.Speed * BaseSpeed * BaseAccel * accelAmountMultiplier;
        float maxVelocity = state.Speed * BaseSpeed * maxVelocityMultiplier;

        float projVel = Vector3.Dot(rb.velocity, accelDir);
        float accelVel = accelAmount * Time.fixedDeltaTime;
        if (projVel + accelVel > maxVelocity) accelVel = maxVelocity - projVel;
        if (accelVel < 0) accelVel = 0;

        return rb.velocity + accelDir * accelVel;
    }

    private bool CalculateIsOnGround() {
        Collider col = gameObject.GetComponent<Collider>();
        int layerMask = LayerMask.GetMask("Room", "Clutter");

        return Physics.CheckCapsule(col.bounds.center,
                                    new Vector3(col.bounds.center.x,
                                                col.bounds.min.y - 0.005f,
                                                col.bounds.center.z),
                                    0.1f,
                                    layerMask,
                                    QueryTriggerInteraction.Ignore);
    }

    private void DoLook() {
        if (thirdPerson) {
            float lookAngle = SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].camera.transform.eulerAngles.y;
            Vector3 lookEuler = transform.eulerAngles;
            lookEuler.y = lookAngle;
            transform.eulerAngles = lookEuler;
            Debug.Log(lookAngle);
        } else {
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
    }

    private void CheckInteract() {
        Vector3 boxSize = new Vector3(1f, 3f, 1.5f);
        Collider[] overlap = Physics.OverlapBox(transform.position + transform.forward * 2f + transform.up * 1.5f, boxSize, transform.rotation);
        InteractiveObject closest = null;

        // Check for interactive.
        foreach (Collider c in overlap) {
            if (c.tag == "Interactive") {
                InteractiveObject io = c.GetComponentInParent<InteractiveObject>();
                if (io != null && io.CanHighlight) {
                    if (closest == null || Vector3.Distance(transform.position, c.transform.position) < Vector3.Distance(transform.position, io.transform.position))
                        closest = io;
                } 
            }
        }

        // Update Highlights.
        if (closest != null) {
            if (ObjectInFocus != null && ObjectInFocus != closest) {
                ObjectInFocus.FocusLost();
                ObjectInFocus = closest;
                ObjectInFocus.FocusGained();
            } else if (ObjectInFocus == null) {
                ObjectInFocus = closest;
                closest.FocusGained();
            }
        } else if (ObjectInFocus != null) {
            ObjectInFocus.FocusLost();
            ObjectInFocus = null;
        }
    }

    private void DoInteract() {
        if (ObjectInFocus != null)
        {
            ObjectInFocus.DoInteract(entity);

            // Clear UI description. 
            if (ObjectInFocus is DroppedItem) { 
                ObjectInFocus = null;
            }
        }
    }

    public void GetPickup(DroppedItemPickup pickup) {
        PlayerGotItem evnt = PlayerGotItem.Create(entity, Bolt.EntityTargets.OnlyOwner);
        evnt.PickupId = pickup.Id;
        evnt.UsesUsed = pickup.UsesUsed;
        evnt.Send();
    }

    public void AssignPlayer(int id) {
        if (entity.isControllerOrOwner) {
            Debug.LogFormat("Assigning player id {0}", id);
            localPlayer = ReInput.players.GetPlayer(id);
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].PlayerId = localPlayer.id;
            localPlayer.isPlaying = true;
        } else {
            Debug.Log("Please only assign local player as networked owner.");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isOwner) return;
        if (other.tag == "Room") {
            DungeonRoom room = other.GetComponent<DungeonRoom>();
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].AddRoomToCamera(other.transform.Find("Focus"), room);
        }
    }

    private void ToggleView() {
        if (!entity.isOwner) return;
        thirdPerson = !thirdPerson;
        if (thirdPerson)
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].SwitchToThirdPerson();
        else
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].SwitchToOverview();
    }

    public override void OnEvent(TeleportPlayer evnt) {
        if (entity.isOwner)
            transform.position = evnt.position;
    }
}
