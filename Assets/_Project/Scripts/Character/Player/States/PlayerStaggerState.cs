using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Combat;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerStaggerState : IState
    {
        private readonly PlayerController _player;
        private float _timer;
        private StaggerType _type;

        public PlayerStaggerState(PlayerController player) => _player = player;
        public void SetType(StaggerType type) => _type = type;

        public void Enter()
        {
            _timer = 0f;
            _player.MoveVelocity = Vector3.zero;
            _player.CurrentDefenseMode = PlayerController.DefenseMode.None;
            _player.PlayStaggerAnimation(_type);
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            float duration = _type == StaggerType.Big ? _player.BigStaggerDuration : _player.StaggerDuration;
            if (_timer >= duration)
                _player.StateMachine.ChangeState(_player.IdleState);
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}