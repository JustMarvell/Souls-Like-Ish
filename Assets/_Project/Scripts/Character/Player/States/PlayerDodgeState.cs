using UnityEngine;
using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerDodgeState : IState
    {
        private const float PlaceholderDuration = 0.4f;

        private readonly PlayerController _player;
        private float _timer;

        public PlayerDodgeState(PlayerController player) => _player = player;

        public void Enter()
        {
            _timer = 0f;
            _player.RootMotionEnabled = true;
            // TODO: trigger dodge animation, enable i-frames via hurtbox once combat core exists.
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= PlaceholderDuration)
                _player.StateMachine.ChangeState(_player.IdleState);
        }

        public void FixedTick() { }

        public void Exit()
        {
            _player.RootMotionEnabled = false;
        }
    }
}