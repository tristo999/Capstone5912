using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemCreationUtil : EditorWindow
{
    public ItemDefinition item;
    public string baseWeapon = "Assets/Prefabs/HeldItems/Weapons/HeldBasicWand.prefab";
    public string baseActive;
    public string basePassive;
    public string baseDroppedWeapon = "Assets/Prefabs/InteractiveObjects/DroppedItems/Weapons/DroppedBasicWand.prefab";
    public string baseDroppedActive;
    public string baseDroppedPassive;

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
        item.Rarity = (ItemDefinition.ItemRarity)EditorGUILayout.EnumPopup(item.Rarity);
        GUILayout.Label("Speed Mod");
        item.SpeedModifier = EditorGUILayout.FloatField(item.SpeedModifier);
        GUILayout.Label("Health Mod");
        item.HealthModifier = EditorGUILayout.FloatField(item.HealthModifier);
        GUILayout.Label("Damage Mod");
        item.DamageModifier = EditorGUILayout.FloatField(item.DamageModifier);
        GUILayout.Label("Fire Rate Mod");
        item.FireRateModifier = EditorGUILayout.FloatField(item.FireRateModifier);
        GUILayout.Label("Projectile Speed Mod");
        item.ProjectileSpeedModifier = EditorGUILayout.FloatField(item.ProjectileSpeedModifier);

        if (GUILayout.Button("Generate Prefabs")) {
            MakeItemPrefabs();
        }
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

    private void MakeItemPrefabs() {
        GameObject heldPre = null;
        GameObject droppedPre = null;
        if (item.Type == ItemDefinition.ItemType.Active) {
            heldPre = PrefabUtility.LoadPrefabContents(baseActive);
            droppedPre = PrefabUtility.LoadPrefabContents(baseDroppedActive);
        } else if (item.Type == ItemDefinition.ItemType.Passive) {
            heldPre = PrefabUtility.LoadPrefabContents(basePassive);
            droppedPre = PrefabUtility.LoadPrefabContents(baseDroppedPassive);
        } else {
            heldPre = PrefabUtility.LoadPrefabContents(baseWeapon);
            droppedPre = PrefabUtility.LoadPrefabContents(baseDroppedWeapon);
        }
        string sanitizedName = item.ItemName.Replace(" ", "");
        heldPre.name = "Held" + sanitizedName;
        droppedPre.name = "Dropped" + sanitizedName;
        item.HeldModel = PrefabUtility.SaveAsPrefabAsset(heldPre, "Assets/Prefabs/HeldItems/" + heldPre.name + ".prefab");
        item.DroppedModel = PrefabUtility.SaveAsPrefabAsset(droppedPre, "Assets/Prefabs/InteractiveObjects/DroppedItems/" + droppedPre.name + ".prefab");
        PrefabUtility.UnloadPrefabContents(heldPre);
        PrefabUtility.UnloadPrefabContents(droppedPre);
    }
}