using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WizardWeapon : HeldItem
{
    public abstract void OnEquip(PlayerMovementController player);
    public abstract void FireDown();
    public abstract void FireHold();
    public abstract void FireRelease();

}
