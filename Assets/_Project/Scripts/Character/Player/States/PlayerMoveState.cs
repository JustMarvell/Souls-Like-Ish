using UnityEngine;
using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerMoveState : IState
    {
        private readonly PlayerController _player;

        public PlayerMoveState(PlayerController player) => _player = player;

        public void Enter() { }

        public void Tick()
        {
            Vector2 input = _player.InputReader.MoveInput;
            if (input.sqrMagnitude < 0.01f)
            {
                _player.StateMachine.ChangeState(_player.IdleState);
                return;
            }

            Vector3 camForward = _player.CameraTransform.forward;
            Vector3 camRight = _player.CameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * input.y + camRight * input.x;
            float speed = _player.InputReader.SprintHeld ? _player.SprintSpeed : _player.MoveSpeed;
            _player.MoveVelocity = moveDir * speed;

            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            _player.transform.rotation = Quaternion.RotateTowards(
                _player.transform.rotation, targetRot, _player.RotationSpeed * Time.deltaTime);
        }

        public void FixedTick() { }

        public void Exit()
        {
            _player.MoveVelocity = Vector3.zero;
        }
    }
}