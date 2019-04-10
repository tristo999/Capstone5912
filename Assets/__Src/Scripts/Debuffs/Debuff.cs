using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff : MonoBehaviour
{
    public abstract string Name { get; set; }

    public abstract float EffectLength { get; set; }

    public float GrantedTime { get; set; }

    public abstract void OnGiven(IPlayerState playerState);

    public abstract void OnRemoved(IPlayerState playerState);

    public abstract void OnUpdate(IPlayerState playerState);
}
