using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : BoltSingletonPrefab<GameMaster>
{

    private int _startFrame;
    private int _spawnedPlayers;
    private int dangerTime;
    private int destructionTime;
    private int criticalTime;
    private int warningTime;
    public Dictionary<int, BoltEntity> players { get; private set; } = new Dictionary<int, BoltEntity>();
    public Dictionary<int, List<DungeonRoom>> RoomLayers = new Dictionary<int, List<DungeonRoom>>();
    public List<BoltEntity> roomsAndClutter = new List<BoltEntity>();
    public int RoomDropTime = 120;
    public int RoomCriticalTime = 115;
    public int RoomDangerTime = 100;
    public int RoomWarningTime = 90;
    public int RoomLayer;
    public int SpawnedPlayers
    {
        get
        {
            return _spawnedPlayers;
        }
        set
        {
            _spawnedPlayers = value;
            if (_spawnedPlayers >= WizardFightPlayerRegistry.Players.Count()) {
                FreezeDistantEntities();
            }
        }
    }
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
    }

    public void FixedUpdate() {
        if (!BoltNetwork.IsServer) return;
        if (RoomLayer > 0) {
            if (GameTime == warningTime)
                SetDestructionStates(DungeonRoom.DestructionState.Warning);
            if (GameTime == dangerTime) 
                SetDestructionStates(DungeonRoom.DestructionState.Danger);
            if (GameTime == criticalTime)
                SetDestructionStates(DungeonRoom.DestructionState.Critical);
            if (GameTime == destructionTime) {
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
            MatchComplete matchCompleteEvnt = MatchComplete.Create();
            matchCompleteEvnt.Winner = LivePlayers.First();
            matchCompleteEvnt.Send();
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

    public void SetDestructionStates(DungeonRoom.DestructionState level) {
        foreach (DungeonRoom room in RoomLayers[RoomLayer]) {
            if (room) {
                room.GetComponent<BoltEntity>().GetState<IDungeonRoom>().DestructionState = (int)level;
            }
        }
    }

    public void DestroyLayer() {
        foreach (DungeonRoom room in RoomLayers[RoomLayer]) {
            room.GetComponent<BoltEntity>().GetState<IDungeonRoom>().DestructionState = (int)DungeonRoom.DestructionState.Destroyed;
        }
        RoomLayer--;
        destructionTime = GameTime + RoomDropTime;
        dangerTime = GameTime + RoomDangerTime;
        warningTime = GameTime + RoomWarningTime;
        criticalTime = GameTime + RoomCriticalTime;
    }

    public void FreezeDistantEntities() {

        foreach (BoltEntity entity in roomsAndClutter) {
            if (entity != null)
            {
                if (players.Any(pair => Vector3.Distance(pair.Value.transform.position, entity.transform.position) < GenerationManager.instance.roomSize * 1.5f))
                {
                    if (entity.isFrozen)
                        entity.Freeze(false);
                }
                else
                {
                    if (!entity.isFrozen)
                        entity.Freeze(true);
                }
            }
        }
    }

}
