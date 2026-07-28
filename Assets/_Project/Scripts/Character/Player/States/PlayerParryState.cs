using SoulsLikeIsh.Core;
using UnityEngine;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerParryState : IState
    {
        private readonly PlayerController _player;
        private float _timer;

        public PlayerParryState(PlayerController player) => _player = player;

        public void Enter()
        {
            _timer = 0f;
            _player.CurrentDefenseMode = PlayerController.DefenseMode.Parrying;
            // TODO : Trigger parry animation via animator once dedidated clip exist.
        }

        public void Tick()
        {
            _timer += Time.deltaTime;

            if (_player.CurrentDefenseMode == PlayerController.DefenseMode.Parrying && _timer >= _player.ParryWindowDuration)
            {
                _player.CurrentDefenseMode = PlayerController.DefenseMode.None; // window closed, punishable recovery
            }

            if (_timer >= _player.ParryWindowDuration + _player.ParryRecoveryDuration)
                _player.StateMachine.ChangeState(_player.IdleState);
        }

        public void FixedTick() { }
        
        public void Exit()
        {
            _player.CurrentDefenseMode = PlayerController.DefenseMode.None;
        }
    }
}