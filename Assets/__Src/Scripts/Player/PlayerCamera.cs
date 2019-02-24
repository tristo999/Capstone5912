using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public new Camera camera { get; private set; }

    public CinemachineTargetGroup targetGroup;

    private Transform currentRoom;

    private void Awake() {
        camera = GetComponentInChildren<Camera>();
        targetGroup = GetComponentInChildren<CinemachineTargetGroup>();
    }

    public void AddRoomToCamera(Transform room) {
        targetGroup.RemoveMember(currentRoom);
        currentRoom = room;
        targetGroup.AddMember(room, 1f, 10f);
    }

    public void AddPlayerToCamera(Transform player) {
        targetGroup.AddMember(player, 1f, 7f);
    }

    public void RemoveTarget(Transform t) {
        targetGroup.RemoveMember(t);
    }

    public void ClearTargets() {
        foreach (CinemachineTargetGroup.Target target in targetGroup.m_Targets) {
            targetGroup.RemoveMember(target.target);
        }
    }
}
