using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChest : Chest
{
    public ItemDefinition ContainedItem;

    public override void OnOpen()
    {
        if (entity.isOwner) {
            Vector3 tossForce = 2400f * transform.forward + 3200f * transform.up;
            SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
            evnt.Position = transform.position + new Vector3(0, 1.5f, 0f);
            evnt.Force = tossForce;
            evnt.ItemId = ContainedItem.ItemId;
            evnt.SpawnerTag = gameObject.tag;
            evnt.Send();
        }
    }
}
