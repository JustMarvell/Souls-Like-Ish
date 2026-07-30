using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Character.Enemy;

namespace SoulsLikeIsh.AI
{
    public class EnemyAttackState : IState
    {
        private readonly EnemyController _enemy;
        private float _timer;
        private bool _hitboxOpened;
        private bool _hitboxClosed;

        public EnemyAttackState(EnemyController enemy) => _enemy = enemy;

        public void Enter()
        {
            _timer = 0f;
            _hitboxOpened = false;
            _hitboxClosed = false;
            _enemy.Agent.isStopped = true;
            _enemy.Agent.updateRotation = false;
            _enemy.Stamina.TrySpend(_enemy.StaminaCostPerAttack);
            _enemy.WeaponHitbox.SetAttack(_enemy.AttackData.Damage, _enemy.AttackData.StaggerPower, _enemy.AttackData.StaggerType);
            _enemy.FacePlayer();
            _enemy.PlayAttackAnimation();
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            var attack = _enemy.AttackData;

            if (!_hitboxOpened && _timer >= attack.ActiveStart)
            {
                _enemy.WeaponHitbox.EnableHitbox();
                _hitboxOpened = true;
            }

            if (!_hitboxClosed && _timer >= attack.ActiveEnd)
            {
                _enemy.WeaponHitbox.DisableHitbox();
                _hitboxClosed = true;
            }

            if (_timer >= attack.Duration)
                _enemy.StateMachine.ChangeState(_enemy.ChaseState);
        }

        public void FixedTick() { }

        public void Exit()
        {
            if (!_hitboxClosed) _enemy.WeaponHitbox.DisableHitbox();
        }
    }
}