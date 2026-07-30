using UnityEngine;
using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerBlockState : IState
    {
        private const float GuardMoveSpeedMultiplier = 0.5f;
        private const float TargetRefreshInterval = 0.25f;

        private readonly PlayerController _player;
        private Transform _strafeTarget;
        private float _targetRefreshTimer;

        public PlayerBlockState(PlayerController player) => _player = player;

        public void Enter()
        {
            _player.CurrentDefenseMode = PlayerController.DefenseMode.Blocking;
            _player.SetBlocking(true);
            _strafeTarget = null;
            _targetRefreshTimer = 0f;
        }

        public void Tick()
        {
            if (!_player.InputReader.BlockHeld)
            {
                _player.StateMachine.ChangeState(_player.IdleState);
                return;
            }

            _targetRefreshTimer -= Time.deltaTime;
            if (_targetRefreshTimer <= 0f)
            {
                _strafeTarget = _player.FindBlockStrafeTarget();
                _targetRefreshTimer = TargetRefreshInterval;
            }

            Vector2 input = _player.InputReader.MoveInput;
            Vector3 moveDir = _player.GetCameraRelativeDirection(input);
            _player.MoveVelocity = moveDir * (_player.MoveSpeed * GuardMoveSpeedMultiplier);

            Vector3 facingDir = GetFacingDirection();
            if (facingDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(facingDir);
                _player.transform.rotation = Quaternion.RotateTowards(
                    _player.transform.rotation, targetRot, _player.RotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 GetFacingDirection()
        {
            if (_strafeTarget != null)
            {
                Vector3 dir = _strafeTarget.position - _player.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f) return dir;
            }

            Vector3 camForward = _player.CameraTransform.forward;
            camForward.y = 0f;
            return camForward;
        }

        public void FixedTick() { }

        public void Exit()
        {
            _player.CurrentDefenseMode = PlayerController.DefenseMode.None;
            _player.MoveVelocity = Vector3.zero;
            _player.SetBlocking(false);
        }
    }
}