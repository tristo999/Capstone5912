using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : NetworkBehaviour
{
    public static GameMaster instance;

    private int _startFrame;
    private int _spawnedPlayers;
    private int dangerTime;
    private int destructionTime;
    private int criticalTime;
    private int warningTime;
    public Dictionary<int, NetworkBehaviour> players { get; private set; } = new Dictionary<int, NetworkBehaviour>();
    public Dictionary<int, List<DungeonRoom>> RoomLayers = new Dictionary<int, List<DungeonRoom>>();
    public List<NetworkBehaviour> roomsAndClutter = new List<NetworkBehaviour>();
    public List<string> WinningOrder = new List<string>();
    public int RoomDropTime = 27;
    public int RoomCriticalTime = 25;
    public int RoomDangerTime = 23;
    public int RoomWarningTime = 20;
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
        }
    }
    public int GameTime
    {
        get
        {
            return (Time.frameCount - _startFrame) / Time.captureFramerate;
        }
    }

    public List<NetworkBehaviour> LivePlayers
    {
        get
        {
            return players.Values.Where(k => k.GetComponent<PlayerStatsController>().Alive).ToList();
        }
    }

    public AudioSource sfxSource;

    private void Awake() {
        if (instance != null) {
            Destroy(this);
            return;
        }
        instance = this;
        sfxSource = gameObject.AddComponent<AudioSource>();
        _startFrame = Time.frameCount;
        destructionTime = GameTime + RoomDropTime;
        dangerTime = GameTime + RoomDangerTime;
        warningTime = GameTime + RoomWarningTime;
        criticalTime = GameTime + RoomCriticalTime;
    }

    public void FixedUpdate() {
        if (!isServer) return;
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

    public void PlayerIdChange(NetworkBehaviour entity, int id) {
        if (!isServer) return;
        if (!players.ContainsValue(entity)) {
            players.Add(id, entity);
        }
    }

    [Command]
    public void CmdTrackPlayerDeath(string playerName) {
        WinningOrder.Insert(0, playerName);
        if (LivePlayers.Count == 1) {
            //Todo Conversion: Call victory screen
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
                room.GetComponent<DungeonRoom>().currentDestructionState = level;
            }
        }
    }

    public void DestroyLayer() {
        foreach (DungeonRoom room in RoomLayers[RoomLayer]) {
            room.GetComponent<DungeonRoom>().currentDestructionState= DungeonRoom.DestructionState.Destroyed;
        }
        RoomLayer--;
        destructionTime = GameTime + RoomDropTime;
        dangerTime = GameTime + RoomDangerTime;
        warningTime = GameTime + RoomWarningTime;
        criticalTime = GameTime + RoomCriticalTime;
    }

}
