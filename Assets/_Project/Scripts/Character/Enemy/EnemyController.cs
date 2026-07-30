using UnityEngine;
using UnityEngine.AI;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Combat;
using SoulsLikeIsh.Character.Shared;
using SoulsLikeIsh.AI;
using SoulsLikeIsh.World.Encounters;

namespace SoulsLikeIsh.Character.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(StaminaComponent))]
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyController : MonoBehaviour, IDamageable, IStaggerable, ILockOnTarget
    {
        [SerializeField] private Transform player;
        [SerializeField] private Animator animator;
        [SerializeField] private Hitbox weaponHitbox;
        [SerializeField] private Transform lockOnPoint;
        [SerializeField] private AttackData attackData;

        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float staminaCostPerAttack = 20f;
        [SerializeField] private float staggerDuration = 0.6f;
        [SerializeField] private LayerMask visionBlockingLayers;

        public Transform Player => player;
        public Hitbox WeaponHitbox => weaponHitbox;
        public Transform LockOnPoint => lockOnPoint != null ? lockOnPoint : transform;
        public bool IsTargetable => !Health.IsDead;
        public AttackData AttackData => attackData;
        public NavMeshAgent Agent { get; private set; }
        public StaminaComponent Stamina { get; private set; }
        public HealthComponent Health { get; private set; }
        public StateMachine StateMachine { get; private set; }
        public Vector3 SpawnPoint { get; private set; }
        public EncounterArea Encounter { get; set; }
        public PoiseComponent Poise { get; private set; }

        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public float StaminaCostPerAttack => staminaCostPerAttack;
        public float StaggerDuration => staggerDuration;

        public EnemyIdleState IdleState { get; private set; }
        public EnemyChaseState ChaseState { get; private set; }
        public EnemyAttackState AttackState { get; private set; }
        public EnemyStaggerState StaggerState { get; private set; }
        public EnemyDeadState DeadState { get; private set; }
        public EnemyReturnState ReturnState { get; private set; }

        private int _hitReactLayer = -1;
        private float _hitReactTimer;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int StaggerHash = Animator.StringToHash("Stagger");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int HitReactHash = Animator.StringToHash("HitReact");

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Stamina = GetComponent<StaminaComponent>();
            Health = GetComponent<HealthComponent>();
            Poise = GetComponent<PoiseComponent>();
            StateMachine = new StateMachine();

            IdleState = new EnemyIdleState(this);
            ChaseState = new EnemyChaseState(this);
            AttackState = new EnemyAttackState(this);
            StaggerState = new EnemyStaggerState(this);
            DeadState = new EnemyDeadState(this);
            ReturnState = new EnemyReturnState(this);
            SpawnPoint = transform.position;

            if (player == null)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) player = playerObj.transform;
            }

            _hitReactLayer = animator != null ? animator.GetLayerIndex("HitReact") : -1;
        }

        private void Start() => StateMachine.ChangeState(IdleState);

        private void OnEnable() => Health.OnDeath += HandleDeath;
        private void OnDisable() => Health.OnDeath -= HandleDeath;

        private void Update()
        {
            StateMachine.Tick();
            TickHitReact();
            if (animator != null)
                animator.SetFloat(SpeedHash, Agent.velocity.magnitude);
        }

        private void FixedUpdate() => StateMachine.FixedTick();

        public bool CanSeePlayer()
        {
            if (player == null) return false;

            Vector3 toPlayer = player.position - transform.position;
            float distance = toPlayer.magnitude;
            if (distance > detectionRange) return false;

            if (Physics.Raycast(transform.position + Vector3.up, toPlayer.normalized, distance, visionBlockingLayers))
                return false;

            return true;
        }

        public float DistanceToPlayer() => player == null ? float.MaxValue : Vector3.Distance(transform.position, player.position);

        public void FacePlayer()
        {
            if (player == null) return;
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f) return;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        public void PlayAttackAnimation() { if (animator != null) animator.SetTrigger(AttackHash); }
        public void PlayStaggerAnimation() { if (animator != null) animator.SetTrigger(StaggerHash); }

        public void PlayHitReaction()
        {
            if (animator == null || _hitReactLayer < 0) return;
            animator.SetLayerWeight(_hitReactLayer, 1f);
            animator.SetTrigger(HitReactHash);
            _hitReactTimer = 0.4f;
        }

        private void TickHitReact()
        {
            if (_hitReactTimer <= 0f) return;
            _hitReactTimer -= Time.deltaTime;
            if (_hitReactTimer <= 0f) animator.SetLayerWeight(_hitReactLayer, 0f);
        }

        public void PlayDeathAnimation() { if (animator != null) animator.SetTrigger(DeathHash); }

        public void TakeDamage(DamageInfo damageInfo)
        {
            Health.TakeDamage(damageInfo.Amount);
            if (Health.IsDead) return;

            if (Poise != null && StateMachine.CurrentState != StaggerState && Poise.ApplyStagger(damageInfo.StaggerPower))
                StateMachine.ChangeState(StaggerState);
            else if (StateMachine.CurrentState != StaggerState)
                PlayHitReaction();
        }

        public void ApplyStagger()
        {
            if (Health.IsDead) return;
            StateMachine.ChangeState(StaggerState);
        }

        private void HandleDeath() => StateMachine.ChangeState(DeadState);

        public void AlertToChase(Transform playerTransform)
        {
            if (Health.IsDead) return;
            if (player == null) player = playerTransform;
            StateMachine.ChangeState(ChaseState);
        }

        public void LeashBack()
        {
            if (Health.IsDead) return;
            StateMachine.ChangeState(ReturnState);
        }
    }
}