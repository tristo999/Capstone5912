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
    public bool inAttackRange;
    public bool inHitRange;
    public Animator enemyAnimator;
    private bool attackStarted;
    public GameObject chest;
    public Transform renderTransform;

    public override void Attached() {
        state.SetTransforms(state.transform, transform, renderTransform);
        enemyAnimator = GetComponentInChildren<Animator>();
        state.SetAnimator(enemyAnimator);

        if (!entity.isOwner) return;
        state.Health = 5f;
        state.AddCallback("Health", HealthChanged);
        state.OnAttack += Attack;
        nav = this.GetComponent<NavMeshAgent>();
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
        nav.isStopped = true;
        attackStarted = true;
    }

    // Update is called once per frame
    public override void SimulateOwner() {
        if (!inDeathAnim)
        {
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

            animationTimer += BoltNetwork.FrameDeltaTime;
        }
        if (isDead)
            BoltNetwork.Destroy(gameObject);
    }
    
    private void HealthChanged() {
        if (state.Health <= 0f && !inDeathAnim) {
            for (int i = 0; i < 1; i++)
            {
                Vector3 tossForce = 3000f * gameObject.transform.forward + 4000f * gameObject.transform.up;
                SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
                evnt.Position = transform.position + new Vector3(0, 1f, 0f);
                evnt.Force = tossForce;
                evnt.ItemId = -1;
                evnt.Send();
            }
            nav.SetDestination(transform.position);
            state.Dying = true;
            //BoltNetwork.Destroy(entity);
        }
    }

    private void CheckAttack() {
        if (attackStarted && animationTimer > animationLength && inAttackRange && !enemyAnimator.IsInTransition(0)) {
            DamageEntity DamageEntity = DamageEntity.Create(currentPlayer.GetComponent<BoltEntity>());
            DamageEntity.Damage = attackDamage;
            DamageEntity.Send();
            attackStarted = false;
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
        } else
        {
            nav.SetDestination(transform.position);
            nav.isStopped = true;
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

    public override void OnEvent(DamageEntity evnt) {
        if (entity.isOwner) {
            state.Health -= evnt.Damage;
        }
        if (evnt.Owner) {
            PlayerUI ui = evnt.Owner.GetComponent<PlayerUI>();
            ui.AddDamageText(evnt.Damage, evnt.HitPosition);
        }
    }
}
