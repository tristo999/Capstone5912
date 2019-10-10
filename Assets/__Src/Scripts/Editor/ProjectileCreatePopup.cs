using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class ProjectileCreatePopup : EditorWindow
{
    private static string itemName = "";
    public bool waitingForCompile;
    public string waitingName;
    public string waitingPath;
    public string waitingType;

    [MenuItem("Wizard Fight/Create/Projectile")]
    static void Init() {
        itemName = "";
        EditorWindow window = CreateInstance<ProjectileCreatePopup>();
        window.ShowPopup();
    }

    private void OnGUI() {
        if (!EditorApplication.isCompiling && waitingForCompile) {
            waitingForCompile = false;
            GameObject item = PrefabUtility.LoadPrefabContents(ItemCreationUtil.TEMPLATE_PROJECTILE_PREFAB);
            item.name = waitingName;
            item.AddComponent(AssetDatabase.LoadAssetAtPath<MonoScript>(waitingType).GetClass());
            PrefabUtility.SaveAsPrefabAsset(item, waitingPath);
            PrefabUtility.UnloadPrefabContents(item);
            Close();
        } else {
            GUILayout.Label("Projectile Creation Wizard", EditorStyles.boldLabel);
            GUILayout.Label("Name:");
            itemName = GUILayout.TextField(itemName);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel")) {
                Close();
            }
            if (GUILayout.Button("Generate")) {
                CreateProjectileFromTemplate();
            }
        }
    }

    public void CreateProjectileFromTemplate() {
        waitingName = itemName;
        waitingPath = "Assets/__Src/Prefabs/Projectiles/" + itemName + ".prefab";
        string templateLocation = Application.dataPath + "/__Src/Scripts/Templates/TemplateProjectile.cs";
        string outputLocation = ItemCreationUtil.PROJECTILE_SCRIPT_OUTPUT_DIR + itemName + ".cs";
        ItemCreationUtil.CopyAndRenameScript(templateLocation, outputLocation);
        waitingType = "Assets/__Src/Scripts/Projectiles/" + itemName + ".cs";
        waitingForCompile = true;
        AssetDatabase.Refresh();
    }
}
