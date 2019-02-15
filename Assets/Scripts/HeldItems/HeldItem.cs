using UnityEngine;

public abstract class HeldItem : MonoBehaviour
{
    public PlayerInventoryController Owner { get; set; }

    public int Id { get; set; } = -1;
}
