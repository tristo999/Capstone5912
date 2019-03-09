using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ItemCreationUtil : EditorWindow
{
    public readonly static string DEFINITION_DIR = "Assets/__Src/ItemDefinitions/";
    public readonly static string TEMPLATE_SCRIPT_DIR = "Assets/__Src/Scripts/Templates/";
    public readonly static string HELD_ITEM_OUTPUT_DIR = "Assets/__Src/Prefabs/HeldItems/";
    public readonly static string DROPPED_ITEM_OUTPUT_DIR = "Assets/__Src/Prefabs/InteractiveObjects/DroppedItems/";

    public readonly static string WEAPON_SCRIPT_OUTPUT_DIR = "Assets/__Src/Scripts/HeldItems/Weapons/";
    public readonly static string ACTIVE_SCRIPT_OUTPUT_DIR = "Assets/__Src/Scripts/HeldItems/ActiveItems/";
    public readonly static string PASSIVE_SCRIPT_OUTPUT_DIR = "Assets/__Src/Scripts/HeldItems/PassiveItems/";
    public readonly static string PROJECTILE_SCRIPT_OUTPUT_DIR = "Assets/__Src/Scripts/Projectiles/";

    public readonly static string WEAPON_TEMPLATE_HELD_PREFAB = "Assets/__Src/Prefabs/HeldItems/Weapons/TemplateWeapon.prefab";
    public readonly static string ACTIVE_TEMPLATE_HELD_PREFAB = "Assets/__Src/Prefabs/HeldItems/ActiveItems/TemplateActive.prefab";
    public readonly static string PASSIVE_TEMPLATE_HELD_PREFAB = "Assets/__Src/Prefabs/HeldItems/Passives/TemplatePassive.prefab";
    public readonly static string WEAPON_TEMPLATE_DROPPED_PREFAB = "Assets/__Src/Prefabs/InteractiveObjects/DroppedItems/Weapons/TemplateDroppedWeapon.prefab";
    public readonly static string ACTIVE_TEMPLATE_DROPPED_PREFAB = "Assets/__Src/Prefabs/InteractiveObjects/Droppeditems/ActiveItems/TemplateDroppedActive.prefab";
    public readonly static string PASSIVE_TEMPLATE_DROPPED_PREFAB = "Assets/__Src/Prefabs/InteractiveObjects/DroppedItems/Passives/TemplateDroppedPassive.prefab";

    public readonly static string TEMPLATE_PROJECTILE_PREFAB = "Assets/__Src/Prefabs/Projectiles/TemplateProjectile.prefab";

    public ItemDefinition item;

    public List<string> delayedTemplate = new List<string>();
    public List<string> delayedScript = new List<string>();
    public List<string> delayedSavePath = new List<string>();
    public List<bool> delayedIsHeld = new List<bool>();
    public ItemDefinition delayedItem;

    [MenuItem("Wizard Fight/Item Creation Util")]
    public static void ShowWindow() {
        GetWindow(typeof(ItemCreationUtil));
    }

    private void OnEnable() {
        item = CreateInstance<ItemDefinition>();
    }

    void OnGUI() {
        EvaluateDelayedPrefabs();

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

        if (GUILayout.Button("Create Item")) {
            GenerateItem();
            MakeItemPrefabs();
            
            item = CreateInstance<ItemDefinition>();
        }
    }

    private void GenerateItem() {
        AssetDatabase.CreateAsset(item, DEFINITION_DIR + item.ItemName + ".asset");
        AssetDatabase.SaveAssets();
        ItemMaster iM = new ItemMaster();
        iM.OnEnable();
        ItemDefinition loaded = AssetDatabase.LoadAssetAtPath<ItemDefinition>(DEFINITION_DIR + item.ItemName + ".asset");
        iM.Items.Add(loaded);
        iM.OnDisable();
    }

    private void MakeItemPrefabs() {
        string sanitizedName = item.ItemName.Replace(" ", "");
        string heldName = "Held" + sanitizedName;
        string droppedName = "Dropped" + sanitizedName;
        if (item.Type == ItemDefinition.ItemType.Active) {
            CopyAndRenameScript(TEMPLATE_SCRIPT_DIR + "TemplateActive.cs", ACTIVE_SCRIPT_OUTPUT_DIR + sanitizedName + ".cs");
            DelayCreatePrefab(ACTIVE_TEMPLATE_HELD_PREFAB, HELD_ITEM_OUTPUT_DIR + "ActiveItems/" + heldName + ".prefab", true, ACTIVE_SCRIPT_OUTPUT_DIR + sanitizedName + ".cs");
            DelayCreatePrefab(ACTIVE_TEMPLATE_DROPPED_PREFAB, DROPPED_ITEM_OUTPUT_DIR + "ActiveItems/" + droppedName + ".prefab", false);
        } else if (item.Type == ItemDefinition.ItemType.Passive) {
            CopyAndRenameScript(TEMPLATE_SCRIPT_DIR + "TemplatePassive.cs", PASSIVE_SCRIPT_OUTPUT_DIR + sanitizedName + ".cs");
            DelayCreatePrefab(PASSIVE_TEMPLATE_HELD_PREFAB, HELD_ITEM_OUTPUT_DIR + "Passives/" + heldName + ".prefab", true, PASSIVE_SCRIPT_OUTPUT_DIR + sanitizedName + ".cs");
            DelayCreatePrefab(PASSIVE_TEMPLATE_DROPPED_PREFAB, DROPPED_ITEM_OUTPUT_DIR + "Passives/" + droppedName + ".prefab", false);
        } else {
            CopyAndRenameScript(TEMPLATE_SCRIPT_DIR + "TemplateWeapon.cs", WEAPON_SCRIPT_OUTPUT_DIR + sanitizedName + ".cs");
            DelayCreatePrefab(WEAPON_TEMPLATE_HELD_PREFAB, HELD_ITEM_OUTPUT_DIR + "Weapons/" + heldName + ".prefab", true, WEAPON_SCRIPT_OUTPUT_DIR + sanitizedName + ".cs");
            DelayCreatePrefab(WEAPON_TEMPLATE_DROPPED_PREFAB, DROPPED_ITEM_OUTPUT_DIR + "Weapons/" + droppedName + ".prefab", false);
        }
        AssetDatabase.Refresh();
    }

    private void DelayCreatePrefab(string template, string savePath, bool held, string script = null) {
        delayedTemplate.Add(template);
        delayedSavePath.Add(savePath);
        delayedIsHeld.Add(held);
        delayedScript.Add(script);
        delayedItem = item;
    }

    private void EvaluateDelayedPrefabs() {
        if (!EditorApplication.isCompiling) {
            if (delayedTemplate.Count > 0) {
                for (int i = 0; i < delayedTemplate.Count; i++) {
                    string template = delayedTemplate[i];
                    string script = delayedScript[i];
                    string savePath = delayedSavePath[i];
                    bool isHeld = delayedIsHeld[i];
                    GameObject newPrefab = PrefabUtility.LoadPrefabContents(template);
                    newPrefab.name = name;
                    if (isHeld) {
                        newPrefab.AddComponent(AssetDatabase.LoadAssetAtPath<MonoScript>(script).GetClass());
                        delayedItem.HeldModel = PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath);
                    } else {
                        delayedItem.DroppedModel = PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath);
                    }
                    PrefabUtility.UnloadPrefabContents(newPrefab);
                }
                if (EditorUtility.DisplayDialog("Do Bolt compile?", "Would you like to do a Bolt compile now to add the new prefabs?", "Ye", "Nah")) {
                    BoltMenuItems.UpdatePrefabDatabase();
                }
                delayedTemplate.Clear();
                delayedScript.Clear();
                delayedSavePath.Clear();
                delayedIsHeld.Clear();
            }
        }
    }

    public static void CopyAndRenameScript(string inPath, string outPath) {
        //inPath = Application.dataPath + inPath;
        //outPath = Application.dataPath + outPath;

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