using Mirror;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SyncVar]
    public GameObject OwnerGameObject;
}