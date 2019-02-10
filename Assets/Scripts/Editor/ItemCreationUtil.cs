using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemCreationUtil : EditorWindow
{
    public WizardFightItem item;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Wizard Fight/Item Creation Util")]
    public static void ShowWindow() {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(ItemCreationUtil));
    }

    private void OnEnable() {
        item = CreateInstance<WizardFightItem>();
    }

    void OnGUI() {
        SerializedObject obj = new SerializedObject(item);
        SerializedProperty worldModel = obj.FindProperty("WorldModel");
        SerializedProperty heldModel = obj.FindProperty("HeldModel");

        item.ItemName = EditorGUILayout.TextField(item.ItemName);
        item.ItemDescription = EditorGUILayout.TextField(item.ItemDescription);
        item.Type = (WizardFightItem.ItemType)EditorGUILayout.EnumPopup(item.Type);
        
        EditorGUILayout.PropertyField(worldModel);
        EditorGUILayout.PropertyField(heldModel);

        obj.ApplyModifiedProperties();

        if (item.WorldModel != null) {
            if (item.WorldModel.GetComponent<Item>() == null) {
                GUILayout.Label("Error, WorldModel lacks Item");
                item.WorldModel = null;
            } else {
                item.ItemScript = item.WorldModel.GetComponent<Item>();
            }
        }

        if (item.HeldModel != null) {
            if (item.HeldModel.GetComponent<HeldItem>() == null) {
                GUILayout.Label("Error, HeldModel lacks HeldItem");
                item.HeldModel = null;
            } else {
                item.HeldScript = item.HeldModel.GetComponent<HeldItem>();
            }
        }

        if (GUILayout.Button("Create Item")) {
            GenerateItem();
        }
    }

    private void GenerateItem() {
        AssetDatabase.CreateAsset(item, "Assets/Items/" + item.ItemName + ".asset");
        AssetDatabase.SaveAssets();
        ItemMaster iM = new ItemMaster();
        iM.OnEnable();
        WizardFightItem loaded = AssetDatabase.LoadAssetAtPath<WizardFightItem>("Assets/Items/" + item.ItemName + ".asset");
        iM.Items.Add(loaded);
        iM.OnDisable();
        item = CreateInstance<WizardFightItem>();
    }
}