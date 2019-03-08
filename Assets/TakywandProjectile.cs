using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakywandProjectile : Bolt.EntityBehaviour<IProjectileState>
{
    public float damage;
    public float maxSizeScale;
    public float knockback;

    private float aliveFor = 0f;
    private float lifetime = .42f;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
    }

    private void OnTriggerEnter(Collider collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        Rigidbody rigid = collision.GetComponent<Rigidbody>();
        if (rigid) {
            Vector3 force = (GetComponent<Rigidbody>().velocity.normalized + new Vector3(0, .25f, 0)) * knockback;
            rigid.AddForce(force);
        }
        if (collision.tag == "Player" && collision.GetComponent<BoltEntity>() != state.Owner) {
            DamageEntity DamageEntity = DamageEntity.Create(collision.gameObject.GetComponent<BoltEntity>());
            DamageEntity.Damage = damage;
            DamageEntity.Send();
            BoltNetwork.Destroy(gameObject);
        }
    }

    public void FixedUpdate() {
        float scale = .6f + (aliveFor / lifetime) * maxSizeScale;
        transform.localScale = new Vector3(scale, scale, scale);
        if (entity.isOwner) {
            if (aliveFor > lifetime)
                BoltNetwork.Destroy(gameObject);
            aliveFor += Time.fixedDeltaTime;
        }
    }
}
