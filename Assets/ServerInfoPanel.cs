using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerInfoPanel : MonoBehaviour
{
    public string ServerName
    {
        get
        {
            return serverNameText.text;
        }
        set
        {
            serverNameText.text = value;
        }
    }

    public int MaxPlayers
    {
        get
        {
            return maxPlayers;
        }
        set
        {
            maxPlayers = value;
            serverPlayersText.text = currentPlayers + "/" + maxPlayers;
        }
    }

    public int CurrentPlayers
    {
        get
        {
            return currentPlayers;
        }
        set
        {
            currentPlayers = value;
            serverPlayersText.text = currentPlayers + "/" + maxPlayers;
        }
    }

    private int maxPlayers;
    private int currentPlayers;
    private NetworkManager netMan;

    public TextMeshProUGUI serverNameText;
    public TextMeshProUGUI serverPlayersText;
    public Guid guid;

    private void Start() {
        netMan = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void Select() {
        netMan.PanelSelect(guid);
    }
}
