using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : Bolt.EntityEventListener<IEnemyState>
{
    public NavMeshAgent nav;
    public GameObject currentPlayer;
    public Vector3 intPosition;
    private GameObject[] players;
    public enum enemyState
    {
        idle, chasing, attacking, returning
    };
    public enemyState currentState;
    public float roomWidth = 30;
    public float attackTimer = 0f;
    public float attackCooldown = 5f;
    public float animationLength = 1f;
    public float animationTimer = 0f;
    public bool inAttackRange;
    public bool inHitRange;
    // Start is called before the first frame update
    public override void Attached()
    {
        state.SetTransforms(state.transform, transform);
        if (!entity.isOwner) return;
        nav = this.GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
        intPosition = transform.position;
        currentState = enemyState.idle;
        attackTimer = 0f;
        animationTimer = 0f;
        inAttackRange = false;
        inHitRange = false;
    }

    // Update is called once per frame
    public override void SimulateOwner()
    {
        switch (currentState)
        {
            case enemyState.idle:
                findCurrentPlayer();
                if (currentPlayer != null)
                {
                    currentState = enemyState.chasing;
                }
                break;
            case enemyState.chasing:
                findCurrentPlayer();
                if (currentPlayer != null)
                {
                    if (inAttackRange)
                    {
                        if (attackTimer > attackCooldown)
                        {
                            currentState = enemyState.attacking;
                            nav.SetDestination(transform.position);
                            attackTimer = 0;
                        }
                    }
                    else
                    {
                        nav.SetDestination(currentPlayer.transform.position);
                    }
                }
                else
                {
                    currentState = enemyState.returning;
                }
                break;
            case enemyState.attacking:
                if (animationTimer > animationLength)
                {
                    if (inHitRange)
                    {
                        //Do Damage
                    }
                    animationTimer = 0;
                    currentState = enemyState.chasing;
                } else
                {
                    animationTimer += Time.deltaTime;
                }
                break;

            case enemyState.returning:
                findCurrentPlayer();
                if (currentPlayer)
                {
                    currentState = enemyState.chasing;
                }
                else
                {
                    if (transform.position.x == intPosition.x && transform.position.z == intPosition.z)
                    {
                        currentState = enemyState.idle;
                    }
                    else
                    {
                        nav.SetDestination(intPosition);
                    }
                }
                break;
        }
        attackTimer += Time.deltaTime;
    }

    private void findCurrentPlayer()
    {
        currentPlayer = null;
        foreach (GameObject x in players)
        {
            if (x.transform.position.x < intPosition.x + (roomWidth / 2) && x.transform.position.x > intPosition.x - (roomWidth / 2) && x.transform.position.z < (intPosition.z + roomWidth) / 2 && x.transform.position.z > intPosition.z - (roomWidth / 2))
            {
                if (currentPlayer == null)
                {
                    currentPlayer = x;
                }
                else
                {
                    if (Vector3.Distance(transform.position, currentPlayer.transform.position) > Vector3.Distance(transform.position, x.transform.position))
                    {
                        currentPlayer = x;
                    }
                }
            }
        }
    }
}
