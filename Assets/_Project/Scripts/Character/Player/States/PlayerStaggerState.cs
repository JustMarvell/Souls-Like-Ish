using UnityEngine;
using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerStaggerState : IState
    {
        private readonly PlayerController _player;
        private float _timer;

        public PlayerStaggerState(PlayerController player) => _player = player;

        public void Enter()
        {
            _timer = 0f;
            _player.MoveVelocity = Vector3.zero;
            _player.CurrentDefenseMode = PlayerController.DefenseMode.None;
            _player.PlayStaggerAnimation();
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _player.StaggerDuration)
                _player.StateMachine.ChangeState(_player.IdleState);
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}