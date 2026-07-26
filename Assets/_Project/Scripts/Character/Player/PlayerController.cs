using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Input;

namespace SoulsLikeIsh.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Animator animator;

        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float rotationSpeed = 540f;
        [SerializeField] private float gravity = -20f;

        public PlayerInputReader InputReader => inputReader;
        public Transform CameraTransform => cameraTransform;
        public Animator Animator => animator;
        public CharacterController CharacterController { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public float MoveSpeed => moveSpeed;
        public float SprintSpeed => sprintSpeed;
        public float RotationSpeed => rotationSpeed;

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
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick();
        }

        private void OnAnimatorMove()
        {
            // TODO: once root motion animations exist, feed animator.deltaPosition into
            // CharacterController.Move here when RootMotionEnabled is true (Attack/Dodge states).
        }

        private void ApplyGravity()
        {
            if (CharacterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            else
                _verticalVelocity += gravity * Time.deltaTime;
        }

        private void HandleAttack() => StateMachine.ChangeState(AttackState);
        private void HandleDodge() => StateMachine.ChangeState(DodgeState);
    }
}