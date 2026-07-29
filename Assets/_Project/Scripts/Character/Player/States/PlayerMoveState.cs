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

            Vector3 moveDir = _player.GetCameraRelativeDirection(input);
            float speed = _player.InputReader.SprintHeld ? _player.SprintSpeed : _player.MoveSpeed;
            _player.MoveVelocity = moveDir * speed;

            Vector3 facingDir = _player.IsLockedOn ? GetLockOnFacingDir() : moveDir;
            if (facingDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(facingDir);
                _player.transform.rotation = Quaternion.RotateTowards(
                    _player.transform.rotation, targetRot, _player.RotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 GetLockOnFacingDir()
        {
            Vector3 dir = _player.LockOnTarget.position - _player.transform.position;
            dir.y = 0f;
            return dir;
        }

        public void FixedTick() { }

        public void Exit()
        {
            _player.MoveVelocity = Vector3.zero;
        }
    }
}