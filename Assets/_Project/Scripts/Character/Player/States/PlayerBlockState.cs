using SoulsLikeIsh.Core;
using UnityEngine;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerBlockState : IState
    {
        private const float GuardMoveSpeedMultiplier = 0.5f;
        private readonly PlayerController _player;
        public PlayerBlockState(PlayerController player) => _player = player;

        public void Enter()
        {
            _player.CurrentDefenseMode = PlayerController.DefenseMode.Blocking;
            // TODO : raise guard animation bool once Animator controller has a block layer/state.
        }

        public void Tick()
        {
            if (!_player.InputReader.BlockHeld)
            {
                _player.StateMachine.ChangeState(_player.IdleState);
                return;
            }

            Vector2 input = _player.InputReader.MoveInput;
            Vector3 moveDir = _player.GetCameraRelativeDirection(input);
            _player.MoveVelocity = moveDir * (_player.MoveSpeed * GuardMoveSpeedMultiplier);

            if (moveDir.magnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                _player.transform.rotation = Quaternion.RotateTowards(
                    _player.transform.rotation, targetRot, _player.RotationSpeed * Time.deltaTime
                );
            }
        }

        public void FixedTick() { }

        public void Exit()
        {
            _player.CurrentDefenseMode = PlayerController.DefenseMode.None;
            _player.MoveVelocity = Vector3.zero;
        }
    }
}