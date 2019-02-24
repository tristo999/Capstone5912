using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChest : Chest
{
    public override void OnOpen()
    {
        if (entity.isOwner) {
            Vector3 tossForce = 3000f * gameObject.transform.forward + 4000f * gameObject.transform.up;
            SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
            evnt.Position = transform.position + new Vector3(0, 1f, 0f);
            evnt.Force = tossForce;
            evnt.ItemId = -1;
            evnt.Send();
        }
    }
}
