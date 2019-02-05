using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChest : Chest
{
    public override void OnOpen()
    {
        if (entity.isOwner) {
            Vector3 tossForce = 50f * gameObject.transform.forward + 80f * gameObject.transform.up;
            SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
            evnt.Position = transform.position + new Vector3(0, .2f, 0f);
            evnt.Force = tossForce;
            evnt.ItemId = -1;
            evnt.Send();
        }
    }
}
