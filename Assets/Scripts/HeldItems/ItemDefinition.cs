using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/Item", order = 1)]
public class ItemDefinition : ScriptableObject
{
    public enum ItemType { Weapon, Active, Passive }

    public string ItemName = "???";
    public string ItemDescription = "Unknown Item";
    public ItemType Type;

    public GameObject DroppedModel;
    public DroppedItem DroppedScript;

    public GameObject HeldModel;
    public HeldItem HeldScript;
}
