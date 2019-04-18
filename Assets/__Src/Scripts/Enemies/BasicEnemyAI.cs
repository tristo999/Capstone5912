using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : Bolt.EntityEventListener<IEnemyState>
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
    public bool inDeathAnim
    {
        get
        {
            return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dying");
        }
    }
    public bool isDead
    {
        get
        {
            return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death");
        }
    }

    public float roomWidth = 30;
    public float attackTimer = 6f;
    public float attackCooldown = 6f;
    public float animationLength = 3f;
    public float animationTimer = 0f;
    public float attackDamage = 3f;
    public float health = 30f;
    public bool inAttackRange;
    public bool inHitRange;
    public float itemCount = 4;
    private Animator enemyAnimator;
    private bool attackStarted;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
        enemyAnimator = GetComponentInChildren<Animator>();
        state.SetAnimator(enemyAnimator);

        if (!entity.isOwner) return;
        state.Health = health;
        state.AddCallback("Health", HealthChanged);
        state.OnAttack += Attack;
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = true;
        intPosition = transform.position;
        attackTimer = 100f;
        animationTimer = 0f;
        inAttackRange = false;
        inHitRange = false;
        state.Dying = false;
        state.Death = false;
    }

    private void Attack() {
        attackTimer = 0f;
        animationTimer = 0f;
        SetStopped(true);
        attackStarted = true;
    }

    // Update is called once per frame
    public override void SimulateOwner() {
        if (!inDeathAnim) {
            if (players == null || players.Length == 0) {
                players = GameObject.FindGameObjectsWithTag("Player");
            } else {
                // Update the player list another way, it takes nearly 10ms every frame.
                // players = GameObject.FindGameObjectsWithTag("Player");

                currentPlayer = FindCurrentPlayer();
            }
            
            CheckAttack();
            CheckMove();

            if (attackTimer < attackCooldown) attackTimer += BoltNetwork.FrameDeltaTime;

            animationTimer += BoltNetwork.FrameDeltaTime;
        }
        if (isDead) BoltNetwork.Destroy(gameObject);
    }
    
    private void HealthChanged() {
        if (state.Health <= 0f && !inDeathAnim) {
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 tossForce = 500f * transform.forward + 1000f * transform.up;
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                if (i == 0) 
                    evnt.Position = transform.position + new Vector3(0, 1f, 0f);
                else if ( i == 1)
                    evnt.Position = transform.position + new Vector3(-1, 1f, 1f);
                else if ( i == 2)
                    evnt.Position = transform.position + new Vector3(-1, 1f, 0f);
                else if (i == 3)
                    evnt.Position = transform.position + new Vector3(0, 1f, -1f);
                evnt.Force = tossForce;
                evnt.ItemId = -1;
                evnt.SpawnerTag = gameObject.tag;
                evnt.Send();
            }
            nav.SetDestination(transform.position);
            state.Dying = true;
            //BoltNetwork.Destroy(entity);
        }
    }

    private void CheckAttack() {
        if (attackStarted && animationTimer > animationLength && inAttackRange && !enemyAnimator.IsInTransition(0)) {
            if (currentPlayer) {
                DamageEntity DamageEntity = DamageEntity.Create(currentPlayer.GetComponent<BoltEntity>());
                DamageEntity.Damage = attackDamage;
                DamageEntity.Send();
            }
            attackStarted = false;
        }

        if (currentPlayer && inAttackRange && attackTimer > attackCooldown) {
            state.Attack();
        }
    }

    private void CheckMove() {
        if (inAttackAnim || enemyAnimator.IsInTransition(0)) return;

        if (currentPlayer) {
            nav.enabled = true;
            if (!inAttackRange) {
                nav.SetDestination(currentPlayer.transform.position);
                SetStopped(false);
            } else {
                nav.SetDestination(transform.position);
                SetStopped(true);
            }
        } else {
            if (nav.enabled) {
                nav.SetDestination(intPosition);
                if (nav.remainingDistance <= nav.stoppingDistance) {
                    SetStopped(true);
                    nav.enabled = false;
                } else {
                    SetStopped(false);
                }
            }
        }
    }

    private GameObject FindCurrentPlayer() {
        GameObject pObj = null;
        GameObject closestPlayer = players.Aggregate((curMin, x) => (curMin == null || Vector3.Distance(x.transform.position, transform.position) < Vector3.Distance(curMin.transform.position, transform.position)) ? x : curMin);
        if (Vector3.Distance(closestPlayer.transform.position, intPosition) < roomWidth / 2)
            pObj = closestPlayer;
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
}
