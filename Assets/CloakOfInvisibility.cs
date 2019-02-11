using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakOfInvisibility : WizardActive
{
    public override int Uses { get; set; } = -1;
    public override float Timeout { get; set; } = 10f;

    public Material InvisibleMaterial;
    public Material TransparentMaterial;

    private float timer;
    private List<Material> oldMats;

    public override void ActivateHold() {
        
    }

    public override void ActivateRelease() {
        // This should be moved to update to deactivate after timer.
        MeshRenderer[] renderers = transform.parent.GetComponentsInChildren<MeshRenderer>();
        if (oldMats == null) return;
        for (int i = 0; i < oldMats.Count; i++) {
            renderers[i].material = oldMats[i];
        }
    }

    public override void ActiveDown() {
        Debug.Log("Activated cloak");
        MeshRenderer[] renderers = transform.parent.GetComponentsInChildren<MeshRenderer>();
        oldMats = new List<Material>();
        foreach (MeshRenderer ren in renderers) {
            oldMats.Add(ren.material);
            if (Owner.entity.isOwner) {
                ren.material = TransparentMaterial;
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
