using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : InteractiveObject
{
    public virtual bool TryPickUp()
    {
        OnPickUp();
        return true;
    }

    public virtual void OnPickUp()
    {
        Destroy(gameObject);
    }
}
