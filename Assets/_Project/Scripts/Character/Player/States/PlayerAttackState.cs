using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Combat;
using SoulsLikeIsh.Input;

namespace SoulsLikeIsh.Character.Player
{
    public class PlayerAttackState : IState
    {
        private readonly PlayerController _player;
        private ILockOnTarget _target;
        private float _timer;
        private bool _hitboxOpened;
        private bool _hitboxClosed;
        private bool _bufferedAttack;
        private bool _traversing;

        public PlayerAttackState(PlayerController player) => _player = player;

        public void Enter()
        {
            _target = _player.ActiveAttack.CanWarp ? _player.FindAttackTarget() : null;
            BeginSwing();
        }

        public void CycleTarget()
        {
            var next = TargetFinder.FindNextLockOnTarget(_player.transform.position, _target, _player.TargetSearchRadius, _player.TargetableLayers);
            if (next == null) return;
            _target = next;
            if (_player.IsLockedOn) _player.SwitchLockOnTarget(next);
        }

        private void BeginSwing()
        {
            _timer = 0f;
            _hitboxOpened = false;
            _hitboxClosed = false;
            _bufferedAttack = false;
            _traversing = _target != null;

            if (_target != null) FaceTarget(instant: true);

            _player.MoveVelocity = Vector3.zero;
            _player.RootMotionEnabled = !_traversing;
            _player.WeaponHitbox.SetAttack(_player.ActiveAttack.Damage, _player.ActiveAttack.StaggerPower, _player.ActiveAttack.StaggerType);
        }

        public void Tick()
        {
            if (_traversing) TickTraversal();
            else if (_target != null && _timer <= _player.ActiveAttack.ActiveStart) FaceTarget(instant: false);

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

        private void TickTraversal()
        {
            Vector3 toTarget = _target.LockOnPoint.position - _player.transform.position;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;

            FaceTarget(instant: false);

            if (dist <= _player.AttackStopDistance)
            {
                _traversing = false;
                return;
            }

            float moveDist = Mathf.Min(_player.AttackTraversalSpeed * Time.deltaTime, dist - _player.AttackStopDistance);
            _player.CharacterController.Move(toTarget.normalized * moveDist);
        }

        private void FaceTarget(bool instant)
        {
            Vector3 dir = _target.LockOnPoint.position - _player.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion rot = Quaternion.LookRotation(dir);
            _player.transform.rotation = instant ? rot :
                Quaternion.RotateTowards(_player.transform.rotation, rot, _player.AttackRotationSpeed * Time.deltaTime);
        }

        private void TryAdvance(AttackData attack)
        {
            if (_bufferedAttack && attack.NextCombo != null && _player.Stamina.TrySpend(attack.NextCombo.StaminaCost))
            {
                _player.ActiveAttack = attack.NextCombo;
                _player.PlayAttackAnimation(attack.NextCombo.AnimationState);
                BeginSwing();
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