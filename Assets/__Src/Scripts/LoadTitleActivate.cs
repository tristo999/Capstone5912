using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTitleActivate : MonoBehaviour
{
    private void OnEnable() {
        SceneManager.LoadScene("Title");
    }
}
