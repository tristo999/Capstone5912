using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
public class CommandEvent : UnityEvent<string[]>
{
}

public class Console : MonoBehaviour
{
    public GameObject ConsoleBox;
    public TextMeshProUGUI EntryBox;
    public TextMeshProUGUI HistoryBox;
    public TextMeshProUGUI TitleText;
    public PostProcessVolume postProcessVolume;
    public KeyCode ConsoleKey = KeyCode.BackQuote;
    public KeyCode DismissKey = KeyCode.Escape;
    public KeyCode AcceptKey = KeyCode.Return;
    public List<CommandEvent> commandEvents = new List<CommandEvent>();

    private List<string> previousCommands = new List<string>();
    private string currentCommand = "";
    private ColorGrading grading;
    private LensDistortion distortion;
    private bool raving;
    private Coroutine rave;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        postProcessVolume.profile.TryGetSettings<ColorGrading>(out grading);
        postProcessVolume.profile.TryGetSettings(out distortion);
    }

    // Update is called once per frame
    void Update()
    {
        if (ConsoleBox.activeInHierarchy)
        {
            if (Input.GetKeyDown(DismissKey))
            {
                EntryBox.text = "_";
                ConsoleBox.SetActive(false);
                currentCommand = "";
            }
            else if (Input.GetKeyDown(AcceptKey))
            {
                if (currentCommand.Length > 0)
                {
                    previousCommands.Insert(0, currentCommand);
                    ExecuteCommand(currentCommand);
                    currentCommand = "";
                    EntryBox.text = "_";
                    RefreshHistory();
                }
            } else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (currentCommand.Length > 0)
                {
                    currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
                    EntryBox.text = currentCommand + "_";
                }
            } else if (Input.anyKeyDown)
            {
                currentCommand += Input.inputString;
                EntryBox.text = currentCommand + "_";
            }
        } else if (Input.GetKeyDown(ConsoleKey))
        {
            ConsoleBox.SetActive(true);
            RefreshHistory();
        }
    }

    private void RefreshHistory()
    {
        string history = "";
        for (int i = 9; i > 0; i--)
        {
            if (i < previousCommands.Count) 
                history += previousCommands[i] + "\n";
        }
        if (previousCommands.Count > 0)
            history += previousCommands[0];
        HistoryBox.text = history;
    }

    private void ExecuteCommand(string com)
    {
        string[] comArgs = com.Split(' ');
        string[] args = comArgs.Skip(1).Take(comArgs.Length - 1).ToArray();
        CommandEvent ev = commandEvents.FirstOrDefault(x => x.GetPersistentMethodName(0).ToLower() == comArgs[0].ToLower());
        if (ev != null)
        {
            ev.Invoke(args);
        } else
        {
            previousCommands.Insert(0, "<color=red>Command " + comArgs[0] + " not recognized.</color>");
            RefreshHistory();
        }
    }

    public void SarahKyne(string[] args)
    {
        TitleText.text = "David does\nall the\nwork";
    }

    public void ChangeTitle (string[] args)
    {
        TitleText.text = string.Join(" ", args);
    }

    public void Rave(string[] args)
    {
        if (!raving)
        {
            raving = true;
            rave = StartCoroutine("raveCoroutine");
        } else
        {
            raving = false;
            StopCoroutine(rave);
            grading.colorFilter.value = Color.white;
        }
    }

    public void Fisheye(string[] args)
    {
        distortion.enabled.value = !distortion.enabled.value;
    }

    public IEnumerator raveCoroutine()
    {
        while (true)
        {
            float h;
            float s;
            float v;
            Color.RGBToHSV(grading.colorFilter.value, out h, out s, out v);
            h += .05f;
            s = 1f;
            v = 1f;
            if (h > 1f)
            {
                h = 0f;
            }
            grading.colorFilter.value = Random.ColorHSV(0, 1, .5f, 1f, .8f, 1f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
