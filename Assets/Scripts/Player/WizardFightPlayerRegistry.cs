using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WizardFightPlayerRegistry
{
    static List<WizardFightPlayerObject> players = new List<WizardFightPlayerObject>();

    static WizardFightPlayerObject CreatePlayer(BoltConnection connection) {
        WizardFightPlayerObject player = new WizardFightPlayerObject();
        player.connection = connection;

        if (player.connection != null) {
            player.connection.UserData = player;
        }

        players.Add(player);

        return player;
    }

    public static IEnumerable<WizardFightPlayerObject> AllPlayers
    {
        get { return players; }
    }

    public static WizardFightPlayerObject ServerPlayer
    {
        get { return players.First(player => player.IsServer); }
    }

    public static WizardFightPlayerObject CreateServerPlayer() {
        return CreatePlayer(null);
    }

    public static WizardFightPlayerObject CreateClientPlayer(BoltConnection connection) {
        return CreatePlayer(connection);
    }

    public static WizardFightPlayerObject GetWizardFightPlayer(BoltConnection connection) {
        if (connection == null) {
            return ServerPlayer;
        }

        return (WizardFightPlayerObject)connection.UserData;
    }
}
