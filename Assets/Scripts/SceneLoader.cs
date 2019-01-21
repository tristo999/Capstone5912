using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public GameObject loadingCanvas;
    public List<string> vanityLoadingMessages = new List<string>();

    private bool loadingScene = false;
    private GameObject canvas;
    private TextMeshProUGUI statusText;

    private void Start()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (loadingScene)
        {
            if (Random.Range(0.0f,1.0f) > .99f)
            {
                statusText.text = vanityLoadingMessages[Random.Range(0, vanityLoadingMessages.Count - 1)];
            }
            statusText.color = new Color(statusText.color.r, statusText.color.g, statusText.color.b, Mathf.PingPong(Time.time, 1));
        }
    }

    public void LoadSceneWithScreen(string scene)
    {
        if (loadingScene) return;
        canvas = Instantiate(loadingCanvas);
        statusText = canvas.GetComponentsInChildren<TextMeshProUGUI>()[1];
        statusText.text = vanityLoadingMessages[Random.Range(0, vanityLoadingMessages.Count - 1)];
        loadingScene = true;
        StartCoroutine(LoadScene(scene));
    }

    IEnumerator LoadScene(string scene)
    {
        yield return new WaitForSeconds(5);
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        while (!async.isDone)
        {
            yield return null;
        }
        loadingScene = false;
        Destroy(canvas);
    }
}
