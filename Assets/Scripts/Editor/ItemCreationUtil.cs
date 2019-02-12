using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemCreationUtil : EditorWindow
{
    public ItemDefinition item;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Wizard Fight/Item Creation Util")]
    public static void ShowWindow() {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(ItemCreationUtil));
    }

    private void OnEnable() {
        item = CreateInstance<ItemDefinition>();
    }

    void OnGUI() {
        SerializedObject obj = new SerializedObject(item);
        SerializedProperty droppedModel = obj.FindProperty("DroppedModel");
        SerializedProperty heldModel = obj.FindProperty("HeldModel");

        item.ItemName = EditorGUILayout.TextField(item.ItemName);
        item.ItemDescription = EditorGUILayout.TextField(item.ItemDescription);
        item.Type = (ItemDefinition.ItemType)EditorGUILayout.EnumPopup(item.Type);
        
        EditorGUILayout.PropertyField(droppedModel);
        EditorGUILayout.PropertyField(heldModel);

        obj.ApplyModifiedProperties();

        if (item.DroppedModel != null) {
            if (item.DroppedModel.GetComponent<DroppedItem>() == null) {
                GUILayout.Label("Error, DroppedModel lacks Item");
                item.DroppedModel = null;
            } else {
                item.DroppedScript = item.DroppedModel.GetComponent<DroppedItem>();
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
        AssetDatabase.CreateAsset(item, "Assets/ItemDefinitions/" + item.ItemName + ".asset");
        AssetDatabase.SaveAssets();
        ItemMaster iM = new ItemMaster();
        iM.OnEnable();
        ItemDefinition loaded = AssetDatabase.LoadAssetAtPath<ItemDefinition>("Assets/ItemDefinitions/" + item.ItemName + ".asset");
        iM.Items.Add(loaded);
        iM.OnDisable();
        item = CreateInstance<ItemDefinition>();
    }
}