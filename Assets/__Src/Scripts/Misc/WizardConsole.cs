using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public delegate void CommandHandler(string[] args);

public class WizardConsole : Bolt.GlobalEventListener
{
    class WizardCommand
    {
        public string command { get; private set; }
        public CommandHandler handler { get; private set; }
        public string help { get; private set; }

        public WizardCommand(string command, CommandHandler handler, string help) {
            this.command = command;
            this.handler = handler;
            this.help = help;
        }
    }

    public GameObject ConsoleCanvas;
    public TextMeshProUGUI EntryBox;
    public TextMeshProUGUI HistoryBox;
    public KeyCode ConsoleKey = KeyCode.BackQuote;
    public KeyCode DismissKey = KeyCode.Escape;
    public KeyCode AcceptKey = KeyCode.Return;
    private List<WizardCommand> commands = new List<WizardCommand>();

    private List<string> previousCommands = new List<string>();
    private string currentCommand = "";
    private ColorGrading grading;
    private LensDistortion distortion;
    private bool raving;
    private Coroutine rave;
    private int gettingHistory = -1;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(ConsoleCanvas);
        RegisterCommand("spawn", ItemSpawn, "Spawn item at position");
        RegisterCommand("playerinfo", PlayerInfo, "Get player information.");
        RegisterCommand("listplayers", ListPlayers, "List current players");
        RegisterCommand("tp", Teleport, "Teleport player");
        RegisterCommand("listitems", ListItems, "List all registered items.");
        RegisterCommand("hpmod", ModifyPlayerHealth, "Modify a player's health");
    }

    public void RegisterCommand(string command, CommandHandler handler, string help) {
        commands.Add(new WizardCommand(command, handler, help));
    }

    public void Log(string log) {
        previousCommands.Insert(0, log);
        RefreshHistory();
    }

    private void ExecuteCommand(string com) {
        string[] args = com.Split(' ');
        WizardCommand command = commands.First(c => c.command == args[0].ToLower());
        command.handler.Invoke(args.Skip(1).ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        if (ConsoleCanvas.activeInHierarchy)
        {
            if (Input.GetKeyDown(DismissKey)) {
                EntryBox.text = "_";
                ConsoleCanvas.SetActive(false);
                currentCommand = "";
                gettingHistory = -1;
            } else if (Input.GetKeyDown(AcceptKey)) {
                if (currentCommand.Length > 0) {
                    previousCommands.Insert(0, currentCommand);
                    ExecuteCommand(currentCommand);
                    currentCommand = "";
                    EntryBox.text = "_";
                    gettingHistory = -1;
                    RefreshHistory();
                }
            } else if (Input.GetKeyDown(KeyCode.Backspace)) {
                if (currentCommand.Length > 0) {
                    currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
                    EntryBox.text = currentCommand + "_";
                }
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                if (gettingHistory < previousCommands.Count - 1) {
                    gettingHistory++;
                    currentCommand = previousCommands[gettingHistory];
                    EntryBox.text = currentCommand + "_";
                }
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                if (gettingHistory > 0) {
                    gettingHistory--;
                    currentCommand = previousCommands[gettingHistory];
                } else {
                    currentCommand = "";
                }
                EntryBox.text = currentCommand + "_";
            } else if (Input.anyKeyDown) {
                currentCommand += Input.inputString;
                EntryBox.text = currentCommand + "_";
            } 
        } else if (Input.GetKeyDown(ConsoleKey))
        {
            ConsoleCanvas.SetActive(true);
            RefreshHistory();
        }
    }

    private void RefreshHistory()
    {
        string history = "";
        for (int i = 40; i > 0; i--)
        {
            if (i < previousCommands.Count) 
                history += previousCommands[i] + "\n";
        }
        if (previousCommands.Count > 0)
            history += previousCommands[0];
        HistoryBox.text = history;
    }

    private void ItemSpawn(string[] args) {
        SpawnItem evt = SpawnItem.Create(ItemManager.Instance.entity);
        evt.ItemId = int.Parse(args[0]);
        if (args.Length > 1) {
            if (args.Length == 2) {
                evt.Position = GameMaster.instance.players[int.Parse(args[1])].transform.position + Vector3.up;
            } else {
                float x, y, z;
                x = float.Parse(args[1]);
                y = float.Parse(args[2]);
                z = float.Parse(args[3]);
                evt.Position = new Vector3(x, y, z);
            }
        }
        evt.Send();
    }

    private void PlayerInfo(string[] args) {
        BoltEntity player = GameMaster.instance.players[int.Parse(args[0])];
        IPlayerState playerState = player.GetState<IPlayerState>();
        Log(playerState.PlayerId + "- Name: " + playerState.Name + ", Position: " + player.transform.position + ", Health: " + playerState.Health);
    }

    private void ListPlayers(string[] args) {
        foreach (KeyValuePair<int, BoltEntity> pair in GameMaster.instance.players) {
            Log(pair.Key + "- Name: " + pair.Value.GetState<IPlayerState>().Name);
        }
    }

    private void Teleport(string[] args) {
        BoltEntity player = GameMaster.instance.players[int.Parse(args[0])];
        TeleportPlayer evnt = TeleportPlayer.Create(player);
        float x, y, z;
        x = float.Parse(args[1]);
        y = float.Parse(args[2]);
        z = float.Parse(args[3]);
        evnt.position = new Vector3(x, y, z);
        evnt.Send();
    }

    private void ListItems(string[] args) {
        for (int i = 0; i < ItemManager.Instance.items.Count; i++) {
            ItemDefinition item = ItemManager.Instance.items[i];
            Log(i + " " + item.Rarity.ToString() + ": " + item.ItemName + " - " + item.ItemDescription);
        }
    }

    private void ModifyPlayerHealth(string[] args) {
        BoltEntity player = GameMaster.instance.players[int.Parse(args[0])];
        DamageEntity evnt = DamageEntity.Create(player);
        evnt.Damage = float.Parse(args[1]);
        evnt.Send();
    }
}
