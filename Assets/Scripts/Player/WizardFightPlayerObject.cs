using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardFightPlayerObject
{
    public int numberPlayers;
    public List<BoltEntity> characters = new List<BoltEntity>();
    public BoltConnection connection;

    public bool IsServer
    {
        get { return connection == null; }
    }

    public bool IsClient
    {
        get { return connection != null; }
    }

    public void Spawn(int id) {
        if (!characters[id]) {
            characters[id] = BoltNetwork.Instantiate(BoltPrefabs.Player);

            if (IsServer) {
                characters[id].TakeControl();
            } else {
                characters[id].AssignControl(connection);
            }
        }
    }
}
