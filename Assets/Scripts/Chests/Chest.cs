using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chest : InteractiveObject
{
    public GameObject closedChestModel;
    public GameObject openChestModel;

    public bool IsOpen { get; set; } = false;

    void Start()
    {
        CloseChest();
    }

    public virtual bool TryOpen()
    {
        if (!IsOpen)
        {
            OnOpen();
            return true;
        }
        return false;
    }

    public virtual void OnOpen()
    {
        OpenChest();
    }

    protected void CloseChest()
    {
        GetComponent<MeshFilter>().mesh = closedChestModel.GetComponent<MeshFilter>().sharedMesh;
        IsOpen = false;
    }

    protected void OpenChest()
    {
        GetComponent<MeshFilter>().mesh = openChestModel.GetComponent<MeshFilter>().sharedMesh;
        IsOpen = true;
    }
}
