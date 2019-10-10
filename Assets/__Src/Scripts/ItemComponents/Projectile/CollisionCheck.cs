using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : NetworkBehaviour
{
    public bool CollideWithPlayers;
    public bool CollideWithOwner;
    public bool CollideWithEnvironment;

    public bool ValidCollision(Collision collision) {
        GameObject collidedEntity = collision.gameObject;
        bool playerCheck = collision.gameObject.tag == "Player" && GetComponent<Projectile>().OwnerGameObject != collidedEntity && CollideWithPlayers;
        bool selfCheck = GetComponent<Projectile>().OwnerGameObject == collidedEntity && CollideWithOwner;
        bool envCheck = collision.gameObject.tag != "Player" && CollideWithEnvironment;
        bool rangeColliderCheck = collision.gameObject.tag == "RangeDetector";
        bool projectileCheck = collision.gameObject.tag == "Projectile";
        // if (playerCheck || selfCheck || envCheck || rangeColliderCheck || projectileCheck) Debug.Log($"{collision.gameObject.name} collider| playerCheck: {playerCheck} selfCheck: {selfCheck} envCheck: {envCheck} rangeCol: {rangeColliderCheck} projectileCheck: {projectileCheck}");
        return playerCheck || selfCheck || envCheck || rangeColliderCheck || projectileCheck;
    }

    public bool ValidCollision(Collider collider) {
        GameObject collidedEntity = collider.gameObject;
        bool playerCheck = collider.gameObject.tag != "Player" || GetComponent<Projectile>().OwnerGameObject == collidedEntity || CollideWithPlayers;
        bool selfCheck = GetComponent<Projectile>().OwnerGameObject != collidedEntity || CollideWithOwner;
        bool envCheck = collider.gameObject.tag == "Player" || CollideWithEnvironment;
        bool rangeColliderCheck = collider.gameObject.tag != "RangeDetector";
        bool projectileCheck = collider.gameObject.tag != "Projectile";
        bool roomTriggerCheck = collider.gameObject.tag != "Room";
        bool musicTriggerCheck = collider.gameObject.tag != "Music";
        // if (playerCheck && selfCheck && envCheck && rangeColliderCheck && projectileCheck && roomTriggerCheck) Debug.Log($"{collider.gameObject.name} collision| playerCheck: {!playerCheck} selfCheck: {!selfCheck} envCheck: {!envCheck} rangeCol: {!rangeColliderCheck} projectileCheck: {!projectileCheck}");
        return playerCheck && selfCheck && envCheck && rangeColliderCheck && projectileCheck && roomTriggerCheck && musicTriggerCheck;
    }
}
