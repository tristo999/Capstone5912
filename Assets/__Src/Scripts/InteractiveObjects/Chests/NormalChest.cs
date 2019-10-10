using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChest : Chest
{
    public ItemDefinition ContainedItem;

    public override void OnOpen()
    {
        if (hasAuthority) {
            Vector3 tossForce = 2400f * transform.forward + 3200f * transform.up;
            Vector3 pos = transform.position + new Vector3(0, 1.5f, 0f);
            ItemManager.Instance.CmdSpawn(pos, tossForce, ContainedItem.ItemId, gameObject.tag);
        }
    }
}
