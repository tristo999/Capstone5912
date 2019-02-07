using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WizardActive : HeldItem
{
    private int uses;
    private float timeout;

    public abstract void OnEquip();
    public abstract void Activate();
}
