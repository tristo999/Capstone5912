using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemMaster : EditorWindow
{
    private GameObject ManagerPrefab;
    private ItemManager itemManager;
    public List<ItemDefinition> Items = new List<ItemDefinition>();
    Vector2 scrollPos;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Wizard Fight/Wizard Fight Items")]
    public static void ShowWindow() {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(ItemMaster));
    }

    public void OnEnable() {
        Load();   
    }

    public void OnDisable() {
        Save();
    }

    private void OnLostFocus() {
        UpdatePrefab();
        Save();
    }

    void OnGUI() {
        Load();
        if (GUILayout.Button("Fix Prefabs")) {
            FixPrefabs();
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < Items.Count; i++) {
            if (Items[i] != null) {

                GUIStyle popupStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
                GUIStyle weaponTitleStyle = new GUIStyle(EditorStyles.boldLabel);
                if (Items[i].Rarity != ItemDefinition.ItemRarity.Debug) {
                    popupStyle.normal.textColor = ItemDefinition.RarityColors[(int)Items[i].Rarity];
                    weaponTitleStyle.normal.textColor = ItemDefinition.RarityColors[(int)Items[i].Rarity];
                } else {
                    popupStyle.normal.textColor = Color.black;
                    weaponTitleStyle.normal.textColor = ItemDefinition.RarityColors[(int)Items[i].Rarity];
                }

                EditorGUILayout.LabelField(new GUIContent(Items[i].ItemName, Items[i].ItemDescription), weaponTitleStyle);
                EditorGUILayout.LabelField(Items[i].Type.ToString(), EditorStyles.miniBoldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.Label(AssetPreview.GetAssetPreview(Items[i].DroppedModel));
                GUILayout.BeginVertical();
                Items[i].ItemName = EditorGUILayout.TextField(Items[i].ItemName);
                Items[i].ItemDescription = EditorGUILayout.TextField(Items[i].ItemDescription);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                
                Items[i].Rarity = (ItemDefinition.ItemRarity)EditorGUILayout.EnumPopup(Items[i].Rarity, popupStyle);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                if (GUILayout.Button("Select Dropped Prefab")) {
                    EditorGUIUtility.PingObject(Items[i].DroppedModel);
                    Selection.activeGameObject = Items[i].DroppedModel;
                }
                if (GUILayout.Button("Open Dropped Prefab")) {
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(Items[i].DroppedModel)));
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (GUILayout.Button("Select Held Prefab")) {
                    EditorGUIUtility.PingObject(Items[i].HeldModel);
                    Selection.activeGameObject = Items[i].HeldModel;
                }
                if (GUILayout.Button("Open Held Prefab")) {
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(Items[i].HeldModel)));
                }
                GUILayout.Space(50);
                GUIStyle deleteStyle = new GUIStyle(GUI.skin.GetStyle("button"));
                deleteStyle.normal.textColor = Color.red;
                if (GUILayout.Button("Remove", deleteStyle)) {
                    if (EditorUtility.DisplayDialog("Remove " + Items[i].ItemName + "?", "Are you sure you remove this item " + Items[i].ItemName + " from the item list?", "DO IT", "Nvm")) {
                        Items.Remove(Items[i]);
                        i--;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
            }
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Create New Item")) {
            ItemCreationUtil.ShowWindow();
        }
        SerializedObject obj = new SerializedObject(this);
        SerializedProperty prop = obj.FindProperty("Items");
        GUILayout.Label("Manual Item List", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(prop, true);
        bool noManager = ManagerPrefab == null;
        obj.ApplyModifiedProperties();
        UpdatePrefab();
        Save();
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

    public void Load() {
        string data;
        if (File.Exists(Application.dataPath + "/Resources/WizardFightData/ItemData.json")) {
            data = File.ReadAllText(Application.dataPath + "/Resources/WizardFightData/ItemData.json");
        } else {
            data = EditorPrefs.GetString("WFItems", JsonUtility.ToJson(this, false));
        }
        JsonUtility.FromJsonOverwrite(data, this);
        if (ManagerPrefab == null) {
            ManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/__Src/Prefabs/Managers/ItemManager.prefab");
            itemManager = ManagerPrefab.GetComponent<ItemManager>();
        }
    }

    public void Save() {
        var data = JsonUtility.ToJson(this, false);
        using (FileStream fs = new FileStream(Application.dataPath + "/Resources/WizardFightData/ItemData.json", FileMode.Create)) {
            using (StreamWriter writer = new StreamWriter(fs)) {
                writer.Write(data);
            }
        }
            EditorPrefs.SetString("WFItems", data);
        if (itemManager != null) {
            itemManager.items = Items.ToList();
        }
    }
}