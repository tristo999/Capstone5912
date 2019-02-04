using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    public Material highlightMaterial;

    private Material originalMaterial;

    void Awake()
    {
        originalMaterial = GetComponent<Renderer>().material;
    }

    public virtual void AddHighlight()
    {
        GetComponent<Renderer>().sharedMaterial = highlightMaterial;
    }

    public virtual void RemoveHighlight()
    {
        GetComponent<Renderer>().sharedMaterial = originalMaterial;
    }
}
