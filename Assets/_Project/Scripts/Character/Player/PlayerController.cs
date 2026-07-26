using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Input;
using SoulsLikeIsh.Combat;
using SoulsLikeIsh.Character.Shared;

namespace SoulsLikeIsh.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(StaminaComponent))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private Hitbox weaponHitbox;
        [SerializeField] private AttackData defaultAttack;
        [SerializeField] private float dodgeStaminaCost = 20f;
        [SerializeField] private float dodgeDuration = 0.4f;

        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float rotationSpeed = 540f;
        [SerializeField] private float gravity = -20f;

        public PlayerInputReader InputReader => inputReader;
        public Transform CameraTransform => cameraTransform;
        public Animator Animator => animator;
        public Hitbox WeaponHitbox => weaponHitbox;
        public AttackData CurrentAttack => defaultAttack;
        public StaminaComponent Stamina { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public float MoveSpeed => moveSpeed;
        public float SprintSpeed => sprintSpeed;
        public float RotationSpeed => rotationSpeed;
        public float DodgeDuration => dodgeDuration;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DodgeHash = Animator.StringToHash("Dodge");

        public Vector3 MoveVelocity { get; set; }
        public bool RootMotionEnabled { get; set; }

        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerDodgeState DodgeState { get; private set; }

        private float _verticalVelocity;

        private void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
            Stamina = GetComponent<StaminaComponent>();
            StateMachine = new StateMachine();

            IdleState = new PlayerIdleState(this);
            MoveState = new PlayerMoveState(this);
            AttackState = new PlayerAttackState(this);
            DodgeState = new PlayerDodgeState(this);
        }

        private void Start()
        {
            StateMachine.ChangeState(IdleState);
        }

        private void OnEnable()
        {
            inputReader.OnAttack += HandleAttack;
            inputReader.OnDodge += HandleDodge;
        }

        private void OnDisable()
        {
            inputReader.OnAttack -= HandleAttack;
            inputReader.OnDodge -= HandleDodge;
        }

        private void Update()
        {
            StateMachine.Tick();
            ApplyGravity();
            CharacterController.Move((MoveVelocity + Vector3.up * _verticalVelocity) * Time.deltaTime);

            if (animator != null)
                animator.SetFloat(SpeedHash, MoveVelocity.magnitude);
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
            if (Stamina.TrySpend(defaultAttack.StaminaCost))
                StateMachine.ChangeState(AttackState);
        }

        private void HandleDodge()
        {
            if (Stamina.TrySpend(dodgeStaminaCost))
                StateMachine.ChangeState(DodgeState);
        }

        public void PlayAttackAnimation()
        {
            if (animator != null) animator.SetTrigger(AttackHash);
        }

        public void PlayDodgeAnimation()
        {
            if (animator != null) animator.SetTrigger(DodgeHash);
        }
    }
}