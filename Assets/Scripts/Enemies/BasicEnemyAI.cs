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
    public float attackCooldown = 6f;
    public float animationLength = 3f;
    public float animationTimer = 0f;
    public bool inAttackRange;
    public bool inHitRange;
    public Animator enemyAnimator;
    // Start is called before the first frame update
    public override void Attached()
    {
        state.SetTransforms(state.transform, transform);
        if (!entity.isOwner) return;
        nav = this.GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
        intPosition = transform.position;
        currentState = enemyState.idle;
        attackTimer = 100f;
        animationTimer = 0f;
        inAttackRange = false;
        inHitRange = false;
        enemyAnimator = GetComponentInChildren<Animator>();
        enemyAnimator.SetInteger("Animation", 0);
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
                    enemyAnimator.SetInteger("Animation", 1);
                }
                break;
            case enemyState.chasing:
                findCurrentPlayer();
                if (currentPlayer != null)
                {
                    enemyAnimator.SetInteger("Animation", 1);
                    if (inAttackRange)
                    {
                        if (attackTimer > attackCooldown)
                        {
                            currentState = enemyState.attacking;
                            enemyAnimator.SetInteger("Animation", 2);
                            nav.SetDestination(transform.position);
                            attackTimer = 0;
                        } else
                        {
                            enemyAnimator.SetInteger("Animation", 0);
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
                    enemyAnimator.SetInteger("Animation", 1);
                }
                break;
            case enemyState.attacking:
                if (animationTimer > animationLength)
                {
                    if (inHitRange)
                    {
                        //Do Damage
                    }
                    currentState = enemyState.idle;
                    animationTimer = 0;
                    enemyAnimator.SetInteger("Animation", 0);
                } else
                {
                    animationTimer += Time.deltaTime;
                    enemyAnimator.SetInteger("Animation", 0);
                }
                break;

            case enemyState.returning:
                findCurrentPlayer();
                                        nav.SetDestination(intPosition);
                if (currentPlayer)
                {
                    currentState = enemyState.chasing;
                    enemyAnimator.SetInteger("Animation", 1);
                }
                else
                {
                    if (!nav.pathPending)
                    {
                        if (nav.remainingDistance <= nav.stoppingDistance)
                        {
                            if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                            {
                                currentState = enemyState.idle;
                                enemyAnimator.SetInteger("Animation", 0);
                            }
                        }
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
