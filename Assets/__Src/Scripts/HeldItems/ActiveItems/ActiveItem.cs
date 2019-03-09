using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveItem : HeldItem
{
    public abstract void OnEquip();
    public abstract void ActiveDown();
    public abstract void ActivateHold();
    public abstract void ActivateRelease();
}
