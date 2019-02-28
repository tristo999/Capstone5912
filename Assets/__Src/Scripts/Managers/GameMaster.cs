using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster : BoltSingletonPrefab<GameMaster>
{

    private int _startFrame;
    private int dangerTime;
    private int destructionTime;
    public Dictionary<int, BoltEntity> players { get; private set; } = new Dictionary<int, BoltEntity>();
    public Dictionary<int,List<DungeonRoom>> RoomLayers = new Dictionary<int,List<DungeonRoom>>();
    public int RoomDropTime;
    public int RoomDangerTime;
    public int RoomLayer;
    public int GameTime
    {
        get
        {
            return (BoltNetwork.ServerFrame - _startFrame) / BoltNetwork.FramesPerSecond;
        }
    }

    private void Awake() {
        _startFrame = BoltNetwork.ServerFrame;
        dangerTime = RoomDangerTime;
        destructionTime = RoomDropTime;
    }

    public void PlayerIdChange(BoltEntity entity, int id) {
        if (!BoltNetwork.IsServer) return;
        if (!players.ContainsValue(entity)) {
            players.Add(id, entity);
        }
    }

    public void FixedUpdate() {
        if (!BoltNetwork.IsServer) return;
        if (RoomLayer > 0) {
            if (GameTime == dangerTime) {
                SetLayerDanger();
            }
            if (GameTime == destructionTime) {
                DestroyLayer();
            }
        }
    }

    public void SetRoomLayers(List<DungeonRoom> rooms) {
        foreach (DungeonRoom room in rooms) {
            RoomLayers[room.DistanceFromCenter].Add(room);
        }
    }

    public void SetLayerDanger() {
        foreach (DungeonRoom room in RoomLayers[RoomLayer]) {
            room.GetComponent<BoltEntity>().GetState<IDungeonRoom>().DestructionState = (int)DungeonRoom.DestructionState.Danger;
        }
        dangerTime = GameTime + RoomDangerTime;
    }

    public void DestroyLayer() {
        foreach (DungeonRoom room in RoomLayers[RoomLayer]) {
            room.GetComponent<BoltEntity>().GetState<IDungeonRoom>().DestructionState = (int)DungeonRoom.DestructionState.Destroyed;
        }
        RoomLayer--;
        destructionTime = GameTime + RoomDropTime;
    }
}
