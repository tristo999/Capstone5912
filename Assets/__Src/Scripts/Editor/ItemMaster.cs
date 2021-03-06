﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemMaster : EditorWindow
{
    private GameObject ManagerPrefab;
    private ItemManager itemManager;
    public List<ItemDefinition> Items
    {
        get
        {
            return itemManager.items;
        }
    }
    Vector2 scrollPos;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Wizard Fight/Wizard Fight Items")]
    public static void ShowWindow() {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(ItemMaster));
    }

    public void OnEnable() {
        if (ManagerPrefab == null) {
            ManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/__Src/Prefabs/Managers/ItemManager.prefab");
            itemManager = ManagerPrefab.GetComponent<ItemManager>();
        }
    }

    public void OnDisable() {
        AssetDatabase.SaveAssets();
    }

    void OnGUI() {
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
                if (GUILayout.Button("Select Item Definition")) {
                    EditorGUIUtility.PingObject(Items[i]);
                    Selection.activeObject = Items[i];
                }
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
                Items[i].ItemId = i;
                if (GUILayout.Button("Remove", deleteStyle)) {
                    if (EditorUtility.DisplayDialog("Remove " + Items[i].ItemName + "?", "Are you sure you remove this item " + Items[i].ItemName + " from the item list?", "DO IT", "Nvm")) {
                        Items.Remove(Items[i]);
                        i--;
                        AssetDatabase.SaveAssets();
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
        SerializedObject obj = new SerializedObject(itemManager);
        SerializedProperty prop = obj.FindProperty("items");
        GUILayout.Label("Manager List", EditorStyles.boldLabel);
        if (GUILayout.Button("Open Manager")) {
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(ManagerPrefab)));
        }
        EditorGUILayout.PropertyField(prop, true);
        bool noManager = ManagerPrefab == null;
        obj.ApplyModifiedProperties();
        EditorUtility.SetDirty(ManagerPrefab);
    }
}