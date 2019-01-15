using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Console : MonoBehaviour
{
    public GameObject ConsoleBox;
    public TextMeshProUGUI EntryBox;
    public TextMeshProUGUI HistoryBox;
    public KeyCode ConsoleKey = KeyCode.BackQuote;
    public KeyCode DismissKey = KeyCode.Escape;
    public KeyCode AcceptKey = KeyCode.Return;

    private List<string> previousCommands = new List<string>();
    private string currentCommand = "";

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
}
