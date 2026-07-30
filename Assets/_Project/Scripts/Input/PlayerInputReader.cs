using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulsLikeIsh.Input
{
    [CreateAssetMenu(fileName = "PlayerInputReader", menuName = "SoulsLikeIsh/Input/Player Input Reader")]
    public class PlayerInputReader : ScriptableObject, PlayerControls.IPlayerActions
    {
        public event Action OnAttack;
        public event Action OnDodge;
        public event Action OnParry;
        public event Action OnJump;
        public event Action OnInteract;
        public event Action OnLockOn;
        public event Action OnCycleTarget;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public float ZoomInput { get; private set; }
        public bool BlockHeld { get; private set; }
        public bool SprintHeld { get; private set; }

        private PlayerControls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new PlayerControls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }

        void PlayerControls.IPlayerActions.OnMove(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>();
        void PlayerControls.IPlayerActions.OnLook(InputAction.CallbackContext ctx) => LookInput = ctx.ReadValue<Vector2>();
        void PlayerControls.IPlayerActions.OnZoom(InputAction.CallbackContext ctx) => ZoomInput = ctx.ReadValue<float>();

        void PlayerControls.IPlayerActions.OnAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnAttack?.Invoke();
        }

        void PlayerControls.IPlayerActions.OnDodge(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnDodge?.Invoke();
        }

        void PlayerControls.IPlayerActions.OnParry(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnParry?.Invoke();
        }

        void PlayerControls.IPlayerActions.OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnJump?.Invoke();
        }

        void PlayerControls.IPlayerActions.OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnInteract?.Invoke();
        }

        void PlayerControls.IPlayerActions.OnLockOn(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnLockOn?.Invoke();
        }

        void PlayerControls.IPlayerActions.OnBlock(InputAction.CallbackContext ctx) => BlockHeld = ctx.performed;
        void PlayerControls.IPlayerActions.OnSprint(InputAction.CallbackContext ctx) => SprintHeld = ctx.performed;

        public void OnLookX(InputAction.CallbackContext context)
        {
            
        }

        public void OnLookY(InputAction.CallbackContext context)
        {
            
        }

        void PlayerControls.IPlayerActions.OnCycleTarget(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnCycleTarget?.Invoke();
        }
    }
}