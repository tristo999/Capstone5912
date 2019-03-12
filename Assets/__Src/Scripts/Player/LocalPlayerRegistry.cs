using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LocalPlayerRegistry
{
    static List<BoltEntity> playerEntities = new List<BoltEntity>();

    public static List<Player> Players { get; } = new List<Player>();
    public static List<int> PlayerNumbers { get; } = new List<int>();

    public static IEnumerable<BoltEntity> PlayerEntities
    {
        get
        {
            return playerEntities;
        }
    }

    public static Player PlayerFromId(int id) {
        return Players[PlayerNumbers.IndexOf(id)];
    }

    public static int IdFromPlayer(Player player) {
        return PlayerNumbers[Players.IndexOf(player)];
    }

    public static bool OwnPlayer(int id) {
        return PlayerNumbers.Contains(id);
    }

    public static void SpawnPlayer(SpawnPlayer evnt) {
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, evnt.Position, Quaternion.identity);
        IPlayerState playerState = playerEntity.GetComponent<PlayerMovementController>().state;
        playerEntity.GetComponent<PlayerUI>().ScreenNumber = SplitscreenManager.instance.CreatePlayerCamera(playerEntity.transform);
        playerState.Color = evnt.Color;
        playerState.Name = evnt.Name;
        playerState.PlayerId = evnt.PlayerId;
        playerEntity.TakeControl();
        playerEntities.Add(playerEntity);
    }

    public static void Reset() {
        playerEntities.Clear();
        Players.Clear();
        PlayerNumbers.Clear();
    }
}
