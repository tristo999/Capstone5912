using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : Bolt.EntityEventListener<IEnemyState>
{
    private NavMeshAgent nav;
    private GameObject currentPlayer;
    private Vector3 intPosition;
    private GameObject[] players;
    public bool inAttackAnim
    {
        get
        {
            return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
        }
    }

    public float roomWidth = 30;
    public float attackTimer = 6f;
    public float attackCooldown = 6f;
    public float animationLength = 3f;
    public float animationTimer = 0f;
    public bool inAttackRange;
    public bool inHitRange;
    public Animator enemyAnimator;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
        enemyAnimator = GetComponentInChildren<Animator>();
        state.SetAnimator(enemyAnimator);

        if (!entity.isOwner) return;
        state.OnAttack += Attack;
        nav = this.GetComponent<NavMeshAgent>();
        intPosition = transform.position;
        attackTimer = 100f;
        animationTimer = 0f;
        inAttackRange = false;
        inHitRange = false;
    }

    private void Attack() {
        Debug.Log("Enemy attacking.");
        attackTimer = 0f;
        nav.isStopped = true;
    }

    // Update is called once per frame
    public override void SimulateOwner() {
        if (players == null || players.Length == 0)
            players = GameObject.FindGameObjectsWithTag("Player");
        else {
            players = GameObject.FindGameObjectsWithTag("Player");
            currentPlayer = findCurrentPlayer();
        }
        state.Moving = !nav.isStopped;
        CheckAttack();
        CheckMove();

        if (attackTimer < attackCooldown)
            attackTimer += BoltNetwork.FrameDeltaTime;
    }

    private void CheckAttack() {
        if (!inAttackAnim && inAttackRange) {
            //Do Damage.
        }

        if (currentPlayer && inAttackRange && attackTimer > attackCooldown) {
            state.Attack();
        }
    }

    private void CheckMove() {
        if (inAttackAnim || enemyAnimator.IsInTransition(0)) return;

        if (currentPlayer && !inAttackRange) {
            nav.SetDestination(currentPlayer.transform.position);
            nav.isStopped = false;
        }

        if (!currentPlayer) {
            nav.SetDestination(intPosition);
            if (nav.remainingDistance <= nav.stoppingDistance) {
                nav.isStopped = true;
            } else {
                nav.isStopped = false;
            }
        }
    }

    private GameObject findCurrentPlayer() {
        GameObject pObj = null;
        GameObject closestPlayer = players.Aggregate((curMin, x) => (curMin == null || Vector3.Distance(x.transform.position, transform.position) < Vector3.Distance(curMin.transform.position, transform.position)) ? x : curMin);
        if (Vector3.Distance(closestPlayer.transform.position, intPosition) < roomWidth / 2)
            pObj = closestPlayer;
        return pObj;
    }
}
