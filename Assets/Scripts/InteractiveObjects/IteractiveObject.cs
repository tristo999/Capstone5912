using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    private Shader originalShader;

    // Later: https://youtu.be/SMLbbi8oaO8
    void Awake()
    {
        originalShader = GetComponent<Renderer>().material.shader;
    }

    public virtual void AddHighlight()
    {
        GetComponent<Renderer>().material.shader = null;
    }

    public virtual void RemoveHighlight()
    {
        GetComponent<Renderer>().material.shader = originalShader;
    }
}
