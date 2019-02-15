using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardFightPlayerObject
{
    public BoltConnection connection;

    public bool IsServer
    {
        get { return connection == null; }
    }

    public bool IsClient
    {
        get { return connection != null; }
    }

    public string PlayerName { get; set; }
    public int PlayerId { get; set; }
    public Color PlayerColor { get; set; }

    public BoltEntity Spawn() {
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player);
        playerEntity.transform.position = new Vector3(0, 2, 0);
        IPlayerState playerState = playerEntity.GetComponent<PlayerMovementController>().state; ;
        playerState.Color = PlayerColor;
        playerState.Name = PlayerName;
        playerState.PlayerId = PlayerId;
        return playerEntity;
    }
}
