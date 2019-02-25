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
        if (!canvas) {
            canvas = Instantiate(loadingCanvas);
            statusText = canvas.GetComponentsInChildren<TextMeshProUGUI>()[1];
            statusText.text = vanityLoadingMessages[Random.Range(0, vanityLoadingMessages.Count - 1)];
            DontDestroyOnLoad(canvas);
            canvas.SetActive(false);
        }
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
        if (!canvas) {
            canvas = Instantiate(loadingCanvas);
            statusText = canvas.GetComponentsInChildren<TextMeshProUGUI>()[1];
            statusText.text = vanityLoadingMessages[Random.Range(0, vanityLoadingMessages.Count - 1)];
        }
        loadingScene = true;
        StartCoroutine(LoadScene(scene));
    }

    public void LoadScreenAsync(AsyncOperation async) {
        if (!canvas) {
            canvas = Instantiate(loadingCanvas);
            statusText = canvas.GetComponentsInChildren<TextMeshProUGUI>()[1];
            statusText.text = vanityLoadingMessages[Random.Range(0, vanityLoadingMessages.Count - 1)];
        }
        StartCoroutine(LoadScreenUntilAsync(async));
    }

    public void StartLoadScreen() {
        loadingScene = true;
        if (!canvas) {
            canvas = Instantiate(loadingCanvas);
            statusText = canvas.GetComponentsInChildren<TextMeshProUGUI>()[1];
            statusText.text = vanityLoadingMessages[Random.Range(0, vanityLoadingMessages.Count - 1)];
        }
        canvas.SetActive(true);
    }

    public void CancelLoadScreen() {
        loadingScene = false;
        canvas.SetActive(false);
    }

    public IEnumerator LoadScreenUntilAsync(AsyncOperation async) {
        loadingScene = true;
        while (async == null || !async.isDone) {
            yield return null;
        }
        loadingScene = false;
        canvas.SetActive(false);
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
        canvas.SetActive(false);
    }
}
