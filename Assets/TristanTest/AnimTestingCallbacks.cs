using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "TestNavMesh")]
public class AnimTestingCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene)
    {
        GameObject skele = BoltNetwork.Instantiate(BoltPrefabs.LargeEnemy);
        skele.transform.position = new Vector3(0, 1.71f, 0);
    }
}
