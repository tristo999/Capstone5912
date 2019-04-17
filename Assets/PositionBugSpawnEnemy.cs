﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionBugSpawnEnemy : MonoBehaviour
{
    public List<BoltEntity> enemies = new List<BoltEntity>();

    private void Update() {
        if (BoltNetwork.IsServer && Input.GetKeyDown(KeyCode.Space)) {
            BoltNetwork.Instantiate(enemies[Random.Range(0, enemies.Count)], new Vector3(Random.Range(-10f, 10f), .5f, Random.Range(-10f, -10f)), Quaternion.identity);
        }
    }
}