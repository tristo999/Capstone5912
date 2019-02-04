using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChest : Chest
{
    public override void OnOpen()
    {
        base.OnOpen();

        Vector3 tossForce = 50f * gameObject.transform.forward + 80f * gameObject.transform.up;
        ItemManager.Instance.SpawnItem(gameObject.transform.position + new Vector3(0, 0.2f, 0), tossForce);
    }
}
