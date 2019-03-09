using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ItemCreationUtil : EditorWindow
{
    public readonly static string templateScriptDir = "/__Src/Scripts/Templates/";
    public readonly static string projectileScriptDir = "";
    public readonly static string weaponScriptDir = "";
    public readonly static string activeScriptDir = "";
    public readonly static string passiveScriptDir = "";

    public ItemDefinition item;
    public string baseWeapon = "Assets/__Src/Prefabs/HeldItems/Weapons/HeldBasicWand.prefab";
    public string baseActive = "Assets/__Src/Prefabs/HeldItems/ActiveItems/HeldCloakOfInvisibility.prefab";
    public string basePassive = "Assets/__Src/Prefabs/HeldItems/HeldBasePassive.prefab";
    public string baseDroppedWeapon = "Assets/__Src/Prefabs/InteractiveObjects/DroppedItems/Weapons/DroppedBasicWand.prefab";
    public string baseDroppedActive = "Assets/__Src/Prefabs/InteractiveObjects/Droppeditems/ActiveItems/DroppedCloakOfInvisibility.prefab";
    public string baseDroppedPassive = "Assets/__Src/Prefabs/InteractiveObjects/DroppedItems/DroppedBasePassive.prefab";

    public static string templateProjectile = "Assets/__Src/Prefabs/Projectiles/TemplateProjectile.prefab";

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Wizard Fight/Item Creation Util")]
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
        GUILayout.Label(new GUIContent("Player Stat Modifiers", "Note, these are not the stats for your weapon, those should go on components in your weapon's held item."));
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
        AssetDatabase.CreateAsset(item, "Assets/__Src/ItemDefinitions/" + item.ItemName + ".asset");
        AssetDatabase.SaveAssets();
        ItemMaster iM = new ItemMaster();
        iM.OnEnable();
        ItemDefinition loaded = AssetDatabase.LoadAssetAtPath<ItemDefinition>("Assets/__Src/ItemDefinitions/" + item.ItemName + ".asset");
        iM.Items.Add(loaded);
        iM.OnDisable();
        BoltMenuItems.UpdatePrefabDatabase();
        EditorUtility.DisplayDialog(item.ItemName + " Generated!", "You did it!\nDropped Prefab At: " + AssetDatabase.GetAssetPath(item.DroppedModel) + "\nHeld Prefab At: " + AssetDatabase.GetAssetPath(item.HeldModel), "Gee thanks!");
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
        item.HeldModel = PrefabUtility.SaveAsPrefabAsset(heldPre, "Assets/__Src/Prefabs/HeldItems/" + heldPre.name + ".prefab");
        item.DroppedModel = PrefabUtility.SaveAsPrefabAsset(droppedPre, "Assets/__Src/Prefabs/InteractiveObjects/DroppedItems/" + droppedPre.name + ".prefab");
        PrefabUtility.UnloadPrefabContents(heldPre);
        PrefabUtility.UnloadPrefabContents(droppedPre);
    }

    public static void CopyAndRenameScript(string inPath, string outPath) {
        string templateText = File.ReadAllText(inPath);
        inPath = inPath.Replace('\\', '/');
        outPath = outPath.Replace('\\', '/');
        int index = inPath.LastIndexOf('/');
        int endIndex = inPath.LastIndexOf(".cs");
        string inName = inPath.Substring(index+1, inPath.Length - index - 4);
        index = outPath.LastIndexOf('/');
        endIndex = outPath.LastIndexOf(".cs");
        string outName = outPath.Substring(index+1, outPath.Length - index - 4);

        templateText = templateText.Replace(inName, outName);
        File.WriteAllText(outPath, templateText);
    }
}