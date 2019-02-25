using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public class LobbyProtocol : Bolt.IProtocolToken
{
    public int maxPlayers;
    public int currentPlayers;
    public bool inLobby;
    public string lobbyName;

    public void Read(UdpPacket packet) {
        maxPlayers = packet.ReadInt();
        currentPlayers = packet.ReadInt();
        inLobby = packet.ReadBool();
        lobbyName = packet.ReadString();
    }

    public void Write(UdpPacket packet) {
        packet.WriteInt(maxPlayers);
        packet.WriteInt(currentPlayers);
        packet.WriteBool(inLobby);
        packet.WriteString(lobbyName);
    }
}
