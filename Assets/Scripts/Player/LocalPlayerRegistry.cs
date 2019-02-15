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

    public static void SpawnPlayer(int id) {
        Player player = ReInput.players.GetPlayer(id);
        player.isPlaying = true;
        Players.Add(player);
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player);
        playerEntities.Add(playerEntity);
    }
}
