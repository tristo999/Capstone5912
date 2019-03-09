using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponentUtility : MonoBehaviour
{
    #if UNITY_EDITOR
    void Update()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
            enabled = false;
        }
    }
    #endif
}
