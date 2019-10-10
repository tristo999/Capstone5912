using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInfoMessage : MessageBase
{
    public string lobbyName;
    public List<PlayerInfo> players;
}
