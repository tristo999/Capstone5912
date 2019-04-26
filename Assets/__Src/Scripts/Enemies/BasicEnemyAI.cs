using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : Bolt.EntityEventListener<IEnemyState> {
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

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
        enemyAnimator = GetComponentInChildren<Animator>();
        state.SetAnimator(enemyAnimator);

        if (!entity.isOwner) return;
        state.Health = health;
        state.AddCallback("Health", HealthChanged);
        state.OnAttack += Attack;
        state.Dying = false;
        state.Death = false;

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

    public override void SimulateOwner() {
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

            if (attackCooldownTimer < attackCooldown) attackCooldownTimer += BoltNetwork.FrameDeltaTime;
            attackAnimationHitDelayTimer += BoltNetwork.FrameDeltaTime;
        }
        if (InDeadAnim) BoltNetwork.Destroy(gameObject);
    }
    
    private void HealthChanged() {
        if (state.Health <= 0f && !InDeathAnim && !InDeadAnim) {
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 tossForce = 500f * transform.forward + 1000f * transform.up;
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);

                if (i == 0) evnt.Position = transform.position + new Vector3(0, 1f, 0f);
                else if ( i == 1) evnt.Position = transform.position + new Vector3(-1, 1f, 1f);
                else if ( i == 2) evnt.Position = transform.position + new Vector3(-1, 1f, 0f);
                else if (i == 3) evnt.Position = transform.position + new Vector3(0, 1f, -1f);

                evnt.Force = tossForce;
                evnt.ItemId = -1;
                evnt.SpawnerTag = gameObject.tag;
                evnt.Send();
            }

            if (nav.enabled) nav.SetDestination(transform.position);
            state.Dying = true;
        }
    }

    private void CheckAttack() {
        if (inHitRange && InAttackAnim && !attackHasHit && attackAnimationHitDelayTimer >= attackAnimationHitDelay) {
            // This will likely be buggy with multiple players but leaving it for now.
            if (targetPlayer) {
                DamageEntity DamageEntity = DamageEntity.Create(targetPlayer.GetComponent<BoltEntity>());
                DamageEntity.Damage = attackDamage;
                DamageEntity.Send();

                if (attackKnockback > 0) {
                    KnockbackEntity KnockbackEntity = KnockbackEntity.Create(targetPlayer.GetComponent<BoltEntity>());
                    KnockbackEntity.Force = GetKnockback();
                    KnockbackEntity.Send();
                }

                attackHasHit = true;
            }
        }

        if (inAttackRange && attackCooldownTimer >= attackCooldown && targetPlayer) {
            state.Attack();
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
        state.Moving = !bStopped;
    }

    public override void OnEvent(DamageEntity evnt) {
        if (state.Health > 0) {
            if (entity.isOwner) {
                state.Health -= evnt.Damage;
            }
            if (evnt.Owner) {
                PlayerUI ui = evnt.Owner.GetComponent<PlayerUI>();
                ui.AddDamageText(evnt.Damage, evnt.HitPosition);
            }
        }
    }

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
}
