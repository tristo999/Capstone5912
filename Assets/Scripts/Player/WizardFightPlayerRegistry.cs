using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WizardFightPlayerRegistry
{
    static List<WizardFightPlayerObject> players = new List<WizardFightPlayerObject>();

    public static IEnumerable<WizardFightPlayerObject> Players
    {
        get
        {
            return players;
        }
    }

    public static int NumberConnections
    {
        get
        {
            return players.Select(p => p.connection).Distinct().Count();
        }
    }

    public static void AddLobbyPlayer(BoltConnection c, LobbyPlayer p) {
        WizardFightPlayerObject newPlayer = new WizardFightPlayerObject();
        newPlayer.connection = c;
        newPlayer.PlayerName = p.Name;
        newPlayer.PlayerColor = p.Color;
        newPlayer.PlayerId = p.PlayerId;
        players.Add(newPlayer);
    }

    public static void AddServerPlayer(LobbyPlayer p) {
        WizardFightPlayerObject newPlayer = new WizardFightPlayerObject();
        newPlayer.connection = null;
        newPlayer.PlayerName = p.Name;
        newPlayer.PlayerColor = p.Color;
        newPlayer.PlayerId = p.PlayerId;
        players.Add(newPlayer);
    }
}
