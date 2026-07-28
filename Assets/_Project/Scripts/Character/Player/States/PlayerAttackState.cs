using UnityEngine;
using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerAttackState : IState
    {
        private readonly PlayerController _player;
        private float _timer;
        private bool _hitboxOpened;
        private bool _hitboxClosed;

        public PlayerAttackState(PlayerController player) => _player = player;

        public void Enter()
        {
            _timer = 0f;
            _hitboxOpened = false;
            _hitboxClosed = false;
            _player.MoveVelocity = Vector3.zero;
            _player.RootMotionEnabled = true;
            _player.WeaponHitbox.SetDamage(_player.ActiveAttack.Damage);
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            var attack = _player.ActiveAttack;

            if (!_hitboxOpened && _timer >= attack.ActiveStart)
            {
                _player.WeaponHitbox.EnableHitbox();
                _hitboxOpened = true;
            }

            if (!_hitboxClosed && _timer >= attack.ActiveEnd)
            {
                _player.WeaponHitbox.DisableHitbox();
                _hitboxClosed = true;
            }

            if (_timer >= attack.Duration)
                _player.StateMachine.ChangeState(_player.IdleState);
        }

        public void FixedTick() { }

        public void Exit()
        {
            _player.RootMotionEnabled = false;
            if (!_hitboxClosed) _player.WeaponHitbox.DisableHitbox();
        }
    }
}