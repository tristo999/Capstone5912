using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chest : InteractiveObject
{
    public GameObject closedChestModel;
    public GameObject openChestModel;

    public abstract void OnOpen();

    [Command]
    public override void CmdDoInteract(GameObject bEntity) {
        CmdOpenChest();
    }

    [Command]
    private void CmdOpenChest() {
        ChestOpen();
    }

    [Command]
    private void CmdCloseChest() {
        ChestClose();
    }

    protected void ChestClose()
    {
        GetComponent<MeshFilter>().mesh = closedChestModel.GetComponent<MeshFilter>().sharedMesh;
    }

    protected void ChestOpen()
    {
        GetComponent<MeshFilter>().mesh = openChestModel.GetComponent<MeshFilter>().sharedMesh;
        CanHighlight = false;
        OnOpen();
    }

}
