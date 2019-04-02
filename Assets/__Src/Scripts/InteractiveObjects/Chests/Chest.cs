using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chest : InteractiveObject
{
    public GameObject closedChestModel;
    public GameObject openChestModel;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
        state.AddCallback("Open", OpenChange);
    }

    public abstract void OnOpen();

    public override void DoInteract(BoltEntity bEntity) {
        OpenChest evnt = OpenChest.Create(entity);
        evnt.Send();
    }

    private void OpenChange() {
        if (state.Open) 
            ChestOpen();
    }


    protected void ChestClose()
    {
        GetComponent<MeshFilter>().mesh = closedChestModel.GetComponent<MeshFilter>().sharedMesh;
    }

    protected void ChestOpen()
    {
        GetComponent<MeshFilter>().mesh = openChestModel.GetComponent<MeshFilter>().sharedMesh;
        OnOpen();
    }

    public override void OnEvent(OpenChest evnt) {
        if (entity.isOwner && !state.Open)
            state.Open = !state.Open;
    }

}
