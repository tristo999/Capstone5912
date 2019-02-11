using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WizardActive : HeldItem
{
    public abstract int Uses { get; set; }
    public abstract float Timeout { get; set; }

    public abstract void OnEquip();
    public abstract void ActiveDown();
    public abstract void ActivateHold();
    public abstract void ActivateRelease();
}
