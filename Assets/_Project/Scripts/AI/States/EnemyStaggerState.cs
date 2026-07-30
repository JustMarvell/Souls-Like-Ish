using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Character.Enemy;
using SoulsLikeIsh.Combat;

namespace SoulsLikeIsh.AI
{
    public class EnemyStaggerState : IState
    {
        private readonly EnemyController _enemy;
        private float _timer;
        private StaggerType _type;

        public EnemyStaggerState(EnemyController enemy) => _enemy = enemy;
        public void SetType(StaggerType type) => _type = type;

        public void Enter()
        {
            _timer = 0f;
            _enemy.Agent.isStopped = true;
            _enemy.WeaponHitbox.DisableHitbox();
            _enemy.PlayStaggerAnimation(_type);
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            float duration = _type == StaggerType.Big ? _enemy.BigStaggerDuration : _enemy.StaggerDuration;
            if (_timer >= duration)
                _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}