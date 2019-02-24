using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : HeldItem
{
    public abstract void OnEquip();
    public abstract void FireDown();
    public abstract void FireHold();
    public abstract void FireRelease();

}
