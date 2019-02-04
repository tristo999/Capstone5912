using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : InteractiveObject
{
    private static int nameHoverVerticalOffset = -35;

    private string itemName = "Unnamed";
    private GUIStyle nameGUIStyle = null;

    void OnGUI()
    {
        // Must be called in OnGUI unfortunately.
        if (nameGUIStyle == null)
        {
            // Ref: https://docs.unity3d.com/ScriptReference/GUIStyle.html
            nameGUIStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 16
            };
        }

        if (IsHighlighted())
        {
            nameGUIStyle.fontStyle = FontStyle.Bold;
        } 
        else
        {
            nameGUIStyle.fontStyle = FontStyle.Normal;
        }

        Vector2 namePosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 nameSize = nameGUIStyle.CalcSize(new GUIContent(GetName()));
        Rect nameBounds = new Rect(namePosition.x - nameSize.x / 2, Screen.height - namePosition.y - nameSize.y / 2 + nameHoverVerticalOffset, nameSize.x, nameSize.y);
        GUI.Box(nameBounds, GetName(), nameGUIStyle);
    }

    public virtual bool TryPickUp()
    {
        OnPickUp();
        return true;
    }

    public virtual void OnPickUp()
    {
        Destroy(gameObject);
    }

    public string GetName()
    {
        return itemName;
    }

    protected void SetName(string newName)
    {
        itemName = newName;
    }
}
