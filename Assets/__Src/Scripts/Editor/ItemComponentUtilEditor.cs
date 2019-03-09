using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemComponentUtility))]
public class ItemComponentUtilEditor : Editor
{
    public const string ComponentsFolder = "__Src/Scripts/ItemComponents/";

    public override void OnInspectorGUI() {
        EditorGUILayout.LabelField("Super Cool Wizard Fight Item Util");
        List<MonoScript> itemComponents = LoadAssetsAtPath<MonoScript>(ComponentsFolder + ((ItemComponentUtility)target).gameObject.tag);
        foreach (MonoScript component in itemComponents) {
            EditorGUI.BeginDisabledGroup(((ItemComponentUtility)target).gameObject.GetComponent(component.GetClass()));
            if (GUILayout.Button(component.name)) {
                ((ItemComponentUtility)target).gameObject.AddComponent(component.GetClass());
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    public List<T> LoadAssetsAtPath<T>(string path) {
        List<T> assets = new List<T>();
        string directory = Application.dataPath + "/" + path;
        string[] files = Directory.GetFiles(directory);
        return files.Select(fileName => {
            string temp = fileName.Replace('\\', '/');
            int index = temp.LastIndexOf("/");
            string local = "Assets/" + path;

            if (index > 0) {
                local += temp.Substring(index);
            }
            return AssetDatabase.LoadAssetAtPath(local, typeof(T));
        }
        ).OfType<T>().ToList();
    }

}
