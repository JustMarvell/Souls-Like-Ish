using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Input;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerAttackState : IState
    {
        private readonly PlayerController _player;
        private Transform _target;
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
            _target = _player.FindAttackTarget();

            _player.AttackWarpTarget = _player.ActiveAttack.CanWarp ? _target : null;

            if (_target != null)
            {
                Vector3 dir = _target.position - _player.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    _player.transform.rotation = Quaternion.LookRotation(dir);
            }
            
            _player.MoveVelocity = Vector3.zero;
            _player.RootMotionEnabled = true;
            _player.WeaponHitbox.SetDamage(_player.ActiveAttack.Damage);
        }

        public void Tick()
        {
            if (_target != null && _timer <= _player.ActiveAttack.ActiveStart)
            {
                Vector3 dir = _target.position - _player.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    _player.transform.rotation = Quaternion.RotateTowards(
                        _player.transform.rotation, Quaternion.LookRotation(dir), _player.AttackRotationSpeed * Time.deltaTime);
            }

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
            _player.AttackWarpTarget = null;
        }
    }
}