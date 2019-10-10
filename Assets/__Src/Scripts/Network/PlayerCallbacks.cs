
using Mirror;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

class PlayerCallbacks : NetworkBehaviour
{
    private int waitFor = 0;
    private bool waiting = false;

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

    }

}

