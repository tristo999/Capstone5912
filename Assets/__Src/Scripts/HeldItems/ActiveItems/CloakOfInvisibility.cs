using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActiveTimeout))]
[RequireComponent(typeof(ActiveCooldown))]
public class CloakOfInvisibility : ActiveItem
{
    public Material InvisibleMaterial;
    public Material TransparentMaterial;
    private List<Material> oldMats;
    private List<Material> oldSkinnedMats;

    private ActiveTimeout timeout;
    private ActiveCooldown cooldown;

    public override void ActivateHold() {
        
    }

    public override void ActivateRelease() {
        
    }

    public override void ActiveDown() {
        ActivateCloak();
    }

    public override void OnEquip() {
        GetComponent<Cloth>().capsuleColliders = new CapsuleCollider[] { Owner.GetComponent<CapsuleCollider>() };
        timeout = GetComponent<ActiveTimeout>();
        cooldown = GetComponent<ActiveCooldown>();
        timeout.OnTimeout += DeactivateCloak;
    }

    private void ActivateCloak() {
        if (timeout.InTimeout || !cooldown.Ready) return;
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
        timeout.StartTimeout();
    }

    private void DeactivateCloak() {
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
        cooldown.ResetCooldown();
    }
}
