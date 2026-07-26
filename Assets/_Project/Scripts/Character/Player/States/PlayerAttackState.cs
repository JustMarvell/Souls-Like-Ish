using UnityEngine;
using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerAttackState : IState
    {
        private const float PlaceholderDuration = 0.5f;

        private readonly PlayerController _player;
        private float _timer;

        public PlayerAttackState(PlayerController player) => _player = player;

        public void Enter()
        {
            _timer = 0f;
            _player.MoveVelocity = Vector3.zero;
            _player.RootMotionEnabled = true;
            // TODO: trigger attack animation via Animator, drive exit off animation events
            // and AttackData ScriptableObject once the combat core exists.
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