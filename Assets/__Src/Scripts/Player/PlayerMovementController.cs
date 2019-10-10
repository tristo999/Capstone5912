using Mirror;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    public float BaseSpeed;
    public float BaseAccel;
    public float BaseFriction;
    public bool InputDisabled;
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
            if (hasAuthority) {
                if (value is DroppedItem) {
                    ui.SetItemFullDescription(((DroppedItem)value).Id);
                } else {
                    ui.SetItemFullDescription(-1);
                }
            }
        }
    }

    private PlayerUI ui;
    private Animator anim;
    private NetworkAnimator netAnim;
    private PlayerStatsController statsController;
    private PlayerInventoryController inventoryController;
    private bool thirdPerson;

    public void Awake() {
        if (!isLocalPlayer) return;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
        ui = GetComponent<PlayerUI>();
    }

    public void Update() {
        // Now that we are free from Bolt, this is a normal solution!
        if (!isLocalPlayer || InputDisabled) return;
        DoLook();
        CheckInteract();
        if (localPlayer.GetButtonDown("Interact")) CmdDoInteract();
        if (localPlayer.GetButtonDown("Fire")) CmdFireDown();
        if (localPlayer.GetButton("Fire")) CmdFireHold();
        if (localPlayer.GetButtonUp("Fire")) CmdFireRelease();
        if (localPlayer.GetButtonDown("UseActive")) CmdActiveDown();
        if (localPlayer.GetButton("UseActive")) CmdActiveHold();
        if (localPlayer.GetButtonUp("UseActive")) CmdActiveRelease();
        if (localPlayer.GetButtonDown("ChangeView")) ToggleView();
        //if (localPlayer.GetButtonDown("Pause")) PauseMenu.Instance.TogglePauseMenu(); todo fix me
    }

    public void FixedUpdate() {
        if (!isLocalPlayer) return;

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

    [Command]
    private void CmdPlayerDied() {
        InputDisabled = true;
        anim.SetBool("Dead", true);
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

        anim.SetFloat("ForwardMovement", Vector3.Dot(rb.velocity, transform.forward) / (statsController.Speed * BaseSpeed));
        anim.SetFloat("RightMovement", Vector3.Dot(rb.velocity, transform.right) / (statsController.Speed * BaseSpeed));
    }

    private void UpdateMovementGround(Vector3 movement) {
        rb.velocity = CalculateVelocityFromFriction();
        rb.velocity = CalculateVelocityFromInputAccel(movement, 1, 1);
    }

    private void UpdateMovementAir(Vector3 movement) {
        rb.velocity = CalculateVelocityFromInputAccel(movement, 1 / 4.25f, 1f);
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
        float accelAmount = Mathf.Min(movement.magnitude, 1) * statsController.Speed * BaseSpeed * BaseAccel * accelAmountMultiplier;
        float maxVelocity = statsController.Speed * BaseSpeed * maxVelocityMultiplier;

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

    [Command]
    private void CmdDoInteract() {
        if (ObjectInFocus != null) {
            ObjectInFocus.CmdDoInteract(gameObject);

            // Clear UI description. 
            if (ObjectInFocus is DroppedItem) {
                ObjectInFocus = null;
            }
        }
    }

    [Command]
    private void CmdActiveRelease() {
        inventoryController.ActiveRelease();
    }

    [Command]
    private void CmdActiveHold() {
        inventoryController.ActiveHold();
    }

    [Command]
    private void CmdActiveDown() {
        inventoryController.ActiveDown();
    }

    [Command]
    private void CmdFireRelease() {
        inventoryController.FireRelease();
    }

    [Command]
    private void CmdFireHold() {
        inventoryController.FireHeld();
    }

    [Command]
    private void CmdFireDown() {
        inventoryController.FireDown();
    }

    public void GetPickup(DroppedItemPickup pickup) {
        inventoryController.CmdGiveItem(pickup);
    }

    public void AssignPlayer(int id) {
        if (isLocalPlayer) {
            Debug.LogFormat("Assigning player id {0}", id);
            localPlayer = ReInput.players.GetPlayer(id);
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].PlayerId = localPlayer.id;
            localPlayer.isPlaying = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (isLocalPlayer) return;
        if (other.tag == "Room") {
            DungeonRoom room = other.GetComponent<DungeonRoom>();
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].AddRoomToCamera(other.transform.Find("Focus"), room);
        }
    }

    private void ToggleView() {
        if (!isLocalPlayer) return;
        thirdPerson = !thirdPerson;
        if (thirdPerson)
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].SwitchToThirdPerson();
        else
            SplitscreenManager.instance.playerCameras[ui.ScreenNumber - 1].SwitchToOverview();
    }

    [Command]
    public void CmdTeleportPlayer(Vector3 pos) {
        if (hasAuthority)
            transform.position = pos;
    }

    [Command]
    public void CmdApplyKnockback(Vector3 force) {
        rb.AddForce(force);
    }
}
