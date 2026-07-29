using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Input;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerAttackState : IState
    {
        private readonly PlayerController _player;
        private float _timer;
        private bool _hitboxOpened;
        private bool _hitboxClosed;
        private bool _bufferedAttack;

        public PlayerAttackState(PlayerController player) => _player = player;

        public void Enter()
        {
            _timer = 0f;
            _hitboxOpened = false;
            _hitboxClosed = false;
            _bufferedAttack = false;
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

            if (!_bufferedAttack && _timer >= attack.ComboWindowStart && _timer <= attack.ComboWindowEnd)
            {
                if (_player.Buffer.TryConsume(PlayerAction.Attack, 0.3f))
                    _bufferedAttack = true;
            }

            if (_timer >= attack.Duration)
                TryAdvance(attack);
        }

        private void TryAdvance(SoulsLikeIsh.Combat.AttackData attack)
        {
            if (_bufferedAttack && attack.NextCombo != null && _player.Stamina.TrySpend(attack.NextCombo.StaminaCost))
            {
                _player.ActiveAttack = attack.NextCombo;
                _player.PlayAttackAnimation(attack.NextCombo.AnimationState);
                Enter();
            }
            else
            {
                _player.StateMachine.ChangeState(_player.IdleState);
            }
        }

        public void FixedTick() { }

        public void Exit()
        {
            _player.RootMotionEnabled = false;
            if (!_hitboxClosed) _player.WeaponHitbox.DisableHitbox();
        }
    }
}