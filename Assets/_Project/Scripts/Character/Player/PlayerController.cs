using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Input;
using SoulsLikeIsh.Combat;
using SoulsLikeIsh.Character.Shared;

namespace SoulsLikeIsh.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(StaminaComponent))]
    [RequireComponent(typeof(HealthComponent))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public enum DefenseMode { None, Blocking, Parrying }

        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private Hitbox weaponHitbox;
        [SerializeField] private AttackData defaultAttack;
        [SerializeField] private AttackData counterAttack;
        [SerializeField] private float dodgeStaminaCost = 20f;
        [SerializeField] private float dodgeDuration = 0.4f;
        [SerializeField] private float blockStaminaCost = 15f;
        [SerializeField] private float parryWindowDuration = 0.2f;
        [SerializeField] private float parryRecoveryDuration = 0.3f;
        [SerializeField] private float counterWindowDuration = 1.5f;

        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float rotationSpeed = 540f;
        [SerializeField] private float gravity = -20f;

        public PlayerInputReader InputReader => inputReader;
        public Transform CameraTransform => cameraTransform;
        public Animator Animator => animator;
        public Hitbox WeaponHitbox => weaponHitbox;
        public AttackData ActiveAttack { get; set; }
        public StaminaComponent Stamina { get; private set; }
        public HealthComponent Health { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public StateMachine StateMachine { get; private set; }
        public DefenseMode CurrentDefenseMode { get; set; }
        public bool CounterWindowOpen { get; private set; }

        public float MoveSpeed => moveSpeed;
        public float SprintSpeed => sprintSpeed;
        public float RotationSpeed => rotationSpeed;
        public float DodgeDuration => dodgeDuration;
        public float ParryWindowDuration => parryWindowDuration;
        public float ParryRecoveryDuration => parryRecoveryDuration;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DodgeHash = Animator.StringToHash("Dodge");
        private static readonly int CounterHash = Animator.StringToHash("Counter");
        private static readonly int ParryHash = Animator.StringToHash("Parry");
        private static readonly int BlockHitHash = Animator.StringToHash("BlockHit");
        private static readonly int IsBlockingHash = Animator.StringToHash("IsBlocking");

        public Vector3 MoveVelocity { get; set; }
        public bool RootMotionEnabled { get; set; }

        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerDodgeState DodgeState { get; private set; }
        public PlayerBlockState BlockState { get; private set; }
        public PlayerParryState ParryState { get; private set; }

        private float _verticalVelocity;
        private float _counterWindowTimer;

        private void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
            Stamina = GetComponent<StaminaComponent>();
            Health = GetComponent<HealthComponent>();
            StateMachine = new StateMachine();

            IdleState = new PlayerIdleState(this);
            MoveState = new PlayerMoveState(this);
            AttackState = new PlayerAttackState(this);
            DodgeState = new PlayerDodgeState(this);
            BlockState = new PlayerBlockState(this);
            ParryState = new PlayerParryState(this);
        }

        private void Start()
        {
            StateMachine.ChangeState(IdleState);

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            inputReader.OnAttack += HandleAttack;
            inputReader.OnDodge += HandleDodge;
            inputReader.OnParry += HandleParry;
        }

        private void OnDisable()
        {
            inputReader.OnAttack -= HandleAttack;
            inputReader.OnDodge -= HandleDodge;
            inputReader.OnParry -= HandleParry;
        }

        private void Update()
        {
            StateMachine.Tick();
            ApplyGravity();
            CharacterController.Move((MoveVelocity + Vector3.up * _verticalVelocity) * Time.deltaTime);

            if (animator != null)
                animator.SetFloat(SpeedHash, MoveVelocity.magnitude);

            TickCounterWindow();

            bool inNeutralState = StateMachine.CurrentState == IdleState || StateMachine.CurrentState == MoveState;
            if (inNeutralState && inputReader.BlockHeld)
                StateMachine.ChangeState(BlockState);
        }

        private void TickCounterWindow()
        {
            if (!CounterWindowOpen) return;
            _counterWindowTimer -= Time.deltaTime;
            if (_counterWindowTimer <= 0f) CounterWindowOpen = false;
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick();
        }

        private void OnAnimatorMove()
        {
            if (!RootMotionEnabled || animator == null) return;
            CharacterController.Move(animator.deltaPosition);
            transform.rotation *= animator.deltaRotation;
        }

        private void ApplyGravity()
        {
            if (CharacterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            else
                _verticalVelocity += gravity * Time.deltaTime;
        }

        private void HandleAttack()
        {
            if (CounterWindowOpen)
            {
                CounterWindowOpen = false;
                ActiveAttack = counterAttack != null ? counterAttack : defaultAttack;
                PlayCounterAnimation();
                StateMachine.ChangeState(AttackState);
                return;
            }

            if (StateMachine.CurrentState == AttackState)
            {
                AttackState.BufferAttack();
                return;
            }

            if (Stamina.TrySpend(defaultAttack.StaminaCost))
            {
                ActiveAttack = defaultAttack;
                PlayAttackAnimation(ActiveAttack.AnimationState);
                StateMachine.ChangeState(AttackState);
            }
        }

        private void HandleDodge()
        {
            if (Stamina.TrySpend(dodgeStaminaCost))
                StateMachine.ChangeState(DodgeState);
        }

        private void HandleParry()
        {
            StateMachine.ChangeState(ParryState);
        }

        public void PlayAttackAnimation(string stateName)
        {
            if (animator != null) animator.CrossFadeInFixedTime(stateName, 0.1f);
        }

        public void PlayDodgeAnimation()
        {
            if (animator != null) animator.SetTrigger(DodgeHash);
        }

        public void PlayCounterAnimation()
        {
            if (animator != null) animator.SetTrigger(CounterHash);
        }

        public void PlayParryAnimation()
        {
            if (animator != null) animator.SetTrigger(ParryHash);
        }

        public void PlayBlockHitAnimation()
        {
            if (animator != null) animator.SetTrigger(BlockHitHash);
        }

        public void SetBlocking(bool isBlocking)
        {
            if (animator != null) animator.SetBool(IsBlockingHash, isBlocking);
        }

        public Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            return camForward * input.y + camRight * input.x;
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            switch (CurrentDefenseMode)
            {
                case DefenseMode.Parrying:
                    CounterWindowOpen = true;
                    _counterWindowTimer = counterWindowDuration;
                    damageInfo.Source.GetComponentInParent<IStaggerable>()?.ApplyStagger();
                    break;

                case DefenseMode.Blocking:
                    Stamina.TrySpend(blockStaminaCost);
                    PlayBlockHitAnimation();
                    // TODO: guard-break handling when stamina can't cover the hit.
                    break;

                default:
                    Health.TakeDamage(damageInfo.Amount);
                    break;
            }
        }
    }
}