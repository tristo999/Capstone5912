using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakOfInvisibility : ActiveItem
{
    public override int Uses { get; set; } = -1;
    public override float Timeout { get; set; } = 10f;

    public Material InvisibleMaterial;
    public Material TransparentMaterial;

    private float timer;
    private List<Material> oldMats;
    private List<Material> oldSkinnedMats;

    public override void ActivateHold() {
        
    }

    public override void ActivateRelease() {
        // This should be moved to update to deactivate after timer.
        MeshRenderer[] renderers = transform.parent.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] meshRenderers = transform.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (oldMats == null) return;
        for (int i = 0; i < oldMats.Count; i++) {
            renderers[i].material = oldMats[i];
            renderers[i].gameObject.layer = 0;
        }

        for (int i = 0; i < oldSkinnedMats.Count; i++) {
            meshRenderers[i].material = oldSkinnedMats[i];
            meshRenderers[i].gameObject.layer = 0;
        }
    }

    public override void ActiveDown() {
        Debug.Log("Activated cloak");
        MeshRenderer[] renderers = transform.parent.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] meshRenderers = transform.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
        oldMats = new List<Material>();
        oldSkinnedMats = new List<Material>();
        foreach (MeshRenderer ren in renderers) {
            oldMats.Add(ren.material);
            if (Owner.entity.isOwner) {
                ren.material = TransparentMaterial;
                ren.gameObject.layer = 7 + Owner.GetComponent<PlayerUI>().ScreenNumber;
            } else {
                ren.material = InvisibleMaterial;
            }
        }

        foreach (SkinnedMeshRenderer ren in meshRenderers) {
            oldSkinnedMats.Add(ren.material);
            if (Owner.entity.isOwner) {
                ren.material = TransparentMaterial;
                ren.gameObject.layer = 7 + Owner.GetComponent<PlayerUI>().ScreenNumber;
            } else {
                ren.material = InvisibleMaterial;
            }
        }
    }

    public override void OnEquip() {
        Debug.Log("Cloak equipped");
        GetComponent<Cloth>().capsuleColliders = new CapsuleCollider[] { Owner.GetComponent<CapsuleCollider>() };
    }

    private void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
        }
    }
}
