using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemMaster : EditorWindow
{
    private GameObject ManagerPrefab;
    private ItemManager itemManager;
    public List<ItemDefinition> Items = new List<ItemDefinition>();

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Wizard Fight/Wizard Fight Items")]
    public static void ShowWindow() {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(ItemMaster));
    }

    public void OnEnable() {
        var data = EditorPrefs.GetString("WFItems", JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);
        if (ManagerPrefab == null) {
            ManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Managers/ItemManager.prefab");
            itemManager = ManagerPrefab.GetComponent<ItemManager>();
        }
    }

    public void OnDisable() {
        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString("WFItems", data);
        if (itemManager != null) {
            itemManager.items = Items.ToList();
        }
    }

    private void OnLostFocus() {
        UpdatePrefab();
    }

    void OnGUI() {
        if (GUILayout.Button("Fix Prefabs")) {
            FixPrefabs();
        }
        SerializedObject obj = new SerializedObject(this);
        SerializedProperty prop = obj.FindProperty("Items");
        GUILayout.Label("Items", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(prop, true);
        bool noManager = ManagerPrefab == null;
        obj.ApplyModifiedProperties();
        UpdatePrefab();
    }

    public void FixPrefabs() {
        foreach (ItemDefinition item in Items) {
            item.HeldScript = item.HeldModel.GetComponent<HeldItem>();
            item.DroppedScript = item.DroppedModel.GetComponent<DroppedItem>();
        }
    }

    private void UpdatePrefab() {
        itemManager.items = Items.ToList();
        EditorUtility.SetDirty(ManagerPrefab);
    }
}