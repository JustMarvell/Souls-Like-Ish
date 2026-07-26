using SoulsLikeIsh.Core;
using UnityEngine;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerIdleState : IState
    {
        private readonly PlayerController _player;
        public PlayerIdleState(PlayerController player) => _player = player;

        public void Enter()
        {
            _player.MoveVelocity = Vector3.zero;
        }

        public void Tick()
        {
            if (_player.InputReader.MoveInput.sqrMagnitude > 0.01f)
                _player.StateMachine.ChangeState(_player.MoveState);
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}