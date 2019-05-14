using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMenu() {
        if (BoltNetwork.IsServer) {
            foreach (BoltConnection client in BoltNetwork.Clients) {
                client.Disconnect();   
            }
            BoltLauncher.Shutdown();
        } else {
            DisconnectPlayer.Create().Send();
        }
        SceneManager.LoadScene(1);
    }
}
