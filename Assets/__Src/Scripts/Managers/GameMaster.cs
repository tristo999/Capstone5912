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
    public int RoomDropTime = 40;
    public int RoomDangerTime = 20;
    public int RoomLayer;
    public int GameTime
    {
        get
        {
            return (BoltNetwork.ServerFrame - _startFrame) / BoltNetwork.FramesPerSecond;
        }
    }

    public List<BoltEntity> LivePlayers
    {
        get
        {
            return players.Values.Where(k => !k.GetState<IPlayerState>().Dead).ToList();
        }
    }

    private void Awake() {
        _startFrame = BoltNetwork.ServerFrame;
        dangerTime = RoomDangerTime;
        destructionTime = RoomDropTime;
    }

    public void FixedUpdate() {
        if (!BoltNetwork.IsServer) return;
        if (RoomLayer > 0) {
            if (GameTime == dangerTime) {
                Debug.Log("ITS DANGER TIME FOR LAYER " + RoomLayer);
                SetLayerDanger();
            }
            if (GameTime == destructionTime) {
                Debug.Log("ITS DESTRUCTION TIME FOR LAYER " + RoomLayer);
                DestroyLayer();
            }
        }
    }

    public void PlayerIdChange(BoltEntity entity, int id) {
        if (!BoltNetwork.IsServer) return;
        if (!players.ContainsValue(entity)) {
            players.Add(id, entity);
            entity.GetState<IPlayerState>().AddCallback("Dead", TrackPlayerDeath);
        }
    }

    private void TrackPlayerDeath(Bolt.IState state, string path, Bolt.ArrayIndices indices) {
        if (LivePlayers.Count == 1) {
            Debug.Log("Hey, a player did a win!");
        }
    }

    public void SetRoomLayers(IEnumerable<DungeonRoom> rooms) {
        foreach (DungeonRoom room in rooms) {
            if (!RoomLayers.ContainsKey(room.DistanceFromCenter)) {
                RoomLayers.Add(room.DistanceFromCenter, new List<DungeonRoom>());
            }
            RoomLayers[room.DistanceFromCenter].Add(room);
            if (room.DistanceFromCenter > RoomLayer)
                RoomLayer = room.DistanceFromCenter;
        }
    }

    public void SetLayerDanger() {
        foreach (DungeonRoom room in RoomLayers[RoomLayer]) {
            room.GetComponent<BoltEntity>().GetState<IDungeonRoom>().DestructionState = (int)DungeonRoom.DestructionState.Danger;
        }
    }

    public void DestroyLayer() {
        foreach (DungeonRoom room in RoomLayers[RoomLayer]) {
            room.GetComponent<BoltEntity>().GetState<IDungeonRoom>().DestructionState = (int)DungeonRoom.DestructionState.Destroyed;
        }
        RoomLayer--;
        destructionTime = GameTime + RoomDropTime;
        dangerTime = GameTime + RoomDangerTime;
    }
}
