using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : Bolt.EntityEventListener<IInteractiveState>
{
    public Material highlightMaterial;

    private Material originalMaterial;
    private bool isHighlighted;
    private Renderer ren;

    public abstract void DoInteract(BoltEntity entity);

    public virtual void FocusGained() {
        AddHighlight();
    }

    public virtual void FocusLost() {
        RemoveHighlight();
    }

    public virtual void AddHighlight()
    {
        originalMaterial = GetComponent<Renderer>().sharedMaterial;
        GetComponent<Renderer>().sharedMaterial = highlightMaterial;
        isHighlighted = true;
    }

    public virtual void RemoveHighlight()
    {
        GetComponent<Renderer>().sharedMaterial = originalMaterial;
        isHighlighted = false;
    }

    public bool IsHighlighted()
    {
        return isHighlighted;
    }
}
