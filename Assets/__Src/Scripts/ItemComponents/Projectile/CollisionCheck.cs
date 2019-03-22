using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : Bolt.EntityBehaviour<IProjectileState>
{
    public bool CollideWithPlayers;
    public bool CollideWithOwner;
    public bool CollideWithEnvironment;

    public bool ValidCollision(Collision collision) {
        BoltEntity collidedEntity = collision.gameObject.GetComponent<BoltEntity>();
        bool playerCheck = collision.gameObject.tag == "Player" && CollideWithPlayers;
        bool selfCheck = state.Owner == collidedEntity && CollideWithOwner;
        bool envCheck = collision.gameObject.tag != "Player" && collision.gameObject.tag != "Projectile" && collision.gameObject.tag != "RangeDetector" && CollideWithEnvironment;
        return playerCheck || selfCheck || envCheck;
    }

    public bool ValidCollision(Collider collider) {
        BoltEntity collidedEntity = collider.gameObject.GetComponent<BoltEntity>();
        bool playerCheck = collider.gameObject.tag != "Player" || CollideWithPlayers;
        bool selfCheck = state.Owner != collidedEntity || CollideWithOwner;
        bool envCheck = collider.gameObject.tag != "Player" && collider.gameObject.tag != "Projectile" && collider.gameObject.tag != "RangeDetector" && CollideWithEnvironment;
        return playerCheck && selfCheck && envCheck;
    }
}
