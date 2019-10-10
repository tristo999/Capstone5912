using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : NetworkBehaviour
{
    public Material highlightMaterial;
    public bool CanHighlight { get; set; } = true;

    private Material originalMaterial;
    private bool isHighlighted;
    private Renderer ren;

    [Command]
    public abstract void CmdDoInteract(GameObject entity);

    public virtual void FocusGained() {
        AddHighlight();
    }

    public virtual void FocusLost() {
        RemoveHighlight();
    }

    public virtual void AddHighlight() {
        if (CanHighlight) {
            originalMaterial = GetComponentInChildren<Renderer>().sharedMaterial;
            GetComponentInChildren<Renderer>().sharedMaterial = highlightMaterial;
            isHighlighted = true;
        }
    }

    public virtual void RemoveHighlight() {
        GetComponentInChildren<Renderer>().sharedMaterial = originalMaterial;
        isHighlighted = false;
    }

    public bool IsHighlighted() {
        return isHighlighted;
    }
}
