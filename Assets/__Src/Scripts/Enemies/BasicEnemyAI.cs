using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/* Todo:
 * 
 * We gonna have to fix animations for enemies, in ripping out bolt I broke all of the state based
 * animation triggers because we used bolt built in animator sync.
 * 
 * <3 David
 * 
 */


public class BasicEnemyAI : NetworkBehaviour {
    public enum AttackDirection {left, right};

    public bool InAttackAnim { 
        get { return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attacking"); }
    }
    public bool InDeathAnim { 
        get { return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dying"); }
    }
    public bool InDeadAnim { 
        get { return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"); }
    }
    
    [SyncVar(hook = nameof(OnHealthChanged))]
    public float health = 30f;
    public float itemCount = 4;
    public float attackCooldown = 6f;
    public float attackDamage = 3f;
    public float attackKnockback = 0f;
    public float attackAnimationHitDelay = 0f;
    public AttackDirection attackDirection = AttackDirection.left;
    [HideInInspector]
    public float roomWidth = 10;
    [HideInInspector]
    public float attackCooldownTimer;
    [HideInInspector]
    public bool inAttackRange;
    [HideInInspector]
    public bool inHitRange;

    private NavMeshAgent nav;
    private GameObject targetPlayer;
    private Vector3 initialPosition;
    private GameObject[] players;
    private Animator enemyAnimator;
    private bool attackHasHit;
    private float attackAnimationHitDelayTimer = 0f;

    public void Awake() {
        enemyAnimator = GetComponentInChildren<Animator>();

        if (!isServer) return;

        nav = GetComponent<NavMeshAgent>();
        nav.enabled = true;
        // nav.updatePosition = false; // TODO: Switch entirely to our own position update to support knockback.
        // nav.updateRotation = false;

        initialPosition = transform.position;
        attackCooldownTimer = float.MaxValue;
        inAttackRange = false;
        inHitRange = false;
    }

    private void Attack() {
        attackCooldownTimer = 0f;
        SetStopped(true);
        attackAnimationHitDelayTimer = 0f;
        attackHasHit = false;
    }

    public void FixedUpdate() {
        if (!InDeathAnim && !InDeadAnim) {
            if (players == null || players.Length == 0) {
                players = GameObject.FindGameObjectsWithTag("Player");
            } else {
                // Update the player list another way, it takes nearly 10ms every frame.
                // players = GameObject.FindGameObjectsWithTag("Player");

                targetPlayer = FindTargetPlayer();
            }
            
            CheckAttack();
            CheckMove();

            if (attackCooldownTimer < attackCooldown) attackCooldownTimer += Time.deltaTime;
            attackAnimationHitDelayTimer += Time.deltaTime;
        }
        if (InDeadAnim) Destroy(gameObject);
    }
    
    private void OnHealthChanged() {
        if (health <= 0f && !InDeathAnim && !InDeadAnim) {
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 tossForce = 500f * transform.forward + 1000f * transform.up;
                Vector3 pos;

                if (i == 0) pos = transform.position + new Vector3(0, 1f, 0f);
                else if ( i == 1) pos = transform.position + new Vector3(-1, 1f, 1f);
                else if ( i == 2) pos = transform.position + new Vector3(-1, 1f, 0f);
                else pos = transform.position + new Vector3(0, 1f, -1f);

                ItemManager.Instance.CmdSpawnRandom(pos, tossForce, tag);
            }

            if (nav.enabled) nav.SetDestination(transform.position);
        }
    }

    private void CheckAttack() {
        if (inHitRange && InAttackAnim && !attackHasHit && attackAnimationHitDelayTimer >= attackAnimationHitDelay) {
            // This will likely be buggy with multiple players but leaving it for now.
            if (targetPlayer) {
                // Todo: That code that makes attacks do damage. <3 David

                attackHasHit = true;
            }
        }

        if (inAttackRange && attackCooldownTimer >= attackCooldown && targetPlayer) {
            // Animation and trigger code goes here! Todo! <3 David
        }
    }

    private void CheckMove() {
        if (InAttackAnim || enemyAnimator.IsInTransition(0)) return;

        if (targetPlayer) {
            nav.enabled = true;
            if (!inAttackRange) {
                nav.SetDestination(targetPlayer.transform.position);
                SetStopped(false);
            } else {
                nav.SetDestination(transform.position);
                SetStopped(true);
            }
        } else {
            if (nav.enabled) {
                nav.SetDestination(initialPosition);
                if (nav.remainingDistance <= nav.stoppingDistance) {
                    SetStopped(true);
                    nav.enabled = false;
                } else {
                    SetStopped(false);
                }
            }
        }
    }

    private GameObject FindTargetPlayer() {
        GameObject pObj = null;
        GameObject closestPlayer = players.Aggregate((curMin, x) => (curMin == null || Vector3.Distance(x.transform.position, transform.position) < Vector3.Distance(curMin.transform.position, transform.position)) ? x : curMin);

        if (closestPlayer != null) {
            bool isInRoom = Mathf.Abs(closestPlayer.transform.position.x - initialPosition.x) < roomWidth / 2 && Mathf.Abs(closestPlayer.transform.position.z - initialPosition.z) < roomWidth / 2; // Manhattan distance.

            bool isVisible = true;
            MeshRenderer[] meshRenderers = closestPlayer.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length > 0 && meshRenderers[0].gameObject.layer != 0) isVisible = false;

            if (isInRoom && isVisible) {
                pObj = closestPlayer;
            }
        }
        return pObj;
    }

    private void SetStopped(bool bStopped) {
        if (nav.enabled) {
            nav.isStopped = bStopped;
        }
        // Todo: Animation trigger <3 Dave
    }

    // Todo: Figure out how to refactor damage. <3 Dave
    /*public override void OnEvent(DamageEntity evnt) {
        if (state.Health > 0) {
            if (entity.isOwner) {
                state.Health -= evnt.Damage;
            }
            if (evnt.Owner) {
                PlayerUI ui = evnt.Owner.GetComponent<PlayerUI>();
                ui.AddDamageText(evnt.Damage, evnt.HitPosition);
            }
        }
    }*/ 

    private Vector3 GetKnockback() {
        Vector3 directionToPlayer = (targetPlayer.transform.position - transform.position).normalized;
        Vector3 knockbackDirection;
        if (attackDirection == AttackDirection.left) {
            knockbackDirection = 0.8f * Vector3.Cross(directionToPlayer, transform.up).normalized + 0.2f * directionToPlayer;
        } else {
            knockbackDirection = -0.6f * Vector3.Cross(directionToPlayer, transform.up).normalized + 0.4f * directionToPlayer;
        }
        return (knockbackDirection + new Vector3(0, 0.7f, 0)).normalized * attackKnockback;
    }

    [Command]
    public void CmdDamageEnemy(float amt) {
        health -= amt;
        // Todo add ui popup
    }
}
