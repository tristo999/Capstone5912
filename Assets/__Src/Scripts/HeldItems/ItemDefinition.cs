using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/Item", order = 1)]
public class ItemDefinition : ScriptableObject
{
    public enum ItemType { Weapon, Active, Passive }
    public enum ItemRarity { Common, Uncommon, Rare, Legendary, Mythic, Ludicrous, Busted, Debug = -1 }
    public static Color[] RarityColors = { Color.white, new Color(0.259f, 0.384f, 0.957f), new Color(1, 0, 1), Color.yellow, Color.red, Color.green, Color.magenta, Color.black };

    public string ItemName = "???";
    public string ItemDescription = "Unknown Item";
    [ReadOnly]
    public int ItemId;
    public ItemType Type;
    public ItemRarity Rarity;

    public GameObject DroppedModel;
    [HideInInspector]
    public DroppedItem DroppedScript;

    public GameObject HeldModel;
    [HideInInspector]
    public HeldItem HeldScript;

    public float SpeedModifier;
    public float HealthModifier;
    public float FireRateModifier;
    public float ProjectileSpeedModifier;
    public float DamageModifier;
}
