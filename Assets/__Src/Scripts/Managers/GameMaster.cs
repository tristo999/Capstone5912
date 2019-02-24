using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster : BoltSingletonPrefab<GameMaster>
{
    public Dictionary<int, BoltEntity> players { get; private set; } = new Dictionary<int, BoltEntity>();

    public void PlayerIdChange(BoltEntity entity, int id) {
        if (!players.ContainsValue(entity)) {
            players.Add(id, entity);
        }
    }
}
