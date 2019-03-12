using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemSpawner : MonoBehaviour
{
    public ItemDefinition.ItemRarity preferredRarity;
    [Range(0f, 1f)]
    public float spawnChance;
}
