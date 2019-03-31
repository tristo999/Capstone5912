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
        bool envCheck = collision.gameObject.tag != "Player" && CollideWithEnvironment;
        bool rangeColliderCheck = collision.gameObject.tag != "RangeDetector";
        bool projectileCheck = collision.gameObject.tag != "Projectile";
        return playerCheck || selfCheck || envCheck || rangeColliderCheck || projectileCheck;
    }

    public bool ValidCollision(Collider collider) {
        
        BoltEntity collidedEntity = collider.gameObject.GetComponent<BoltEntity>();
        bool playerCheck = collider.gameObject.tag != "Player" || CollideWithPlayers;
        bool selfCheck = state.Owner != collidedEntity || CollideWithOwner;
        bool envCheck = collider.gameObject.tag != "Player" ||  CollideWithEnvironment;
        bool rangeColliderCheck = collider.gameObject.tag != "RangeDetector";
        bool projectileCheck = collider.gameObject.tag != "Projectile";
        bool roomTriggerCheck = collider.gameObject.tag != "Room";
        return playerCheck && selfCheck && envCheck && rangeColliderCheck && projectileCheck && roomTriggerCheck;

    }
}
