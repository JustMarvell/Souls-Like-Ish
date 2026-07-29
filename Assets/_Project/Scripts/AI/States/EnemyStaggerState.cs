using UnityEngine;
using SoulsLikeIsh.Core;
using SoulsLikeIsh.Character.Enemy;

namespace SoulsLikeIsh.AI
{
    public class EnemyStaggerState : IState
    {
        private readonly EnemyController _enemy;
        private float _timer;

        public EnemyStaggerState(EnemyController enemy) => _enemy = enemy;

        public void Enter()
        {
            _timer = 0f;
            _enemy.Agent.isStopped = true;
            _enemy.WeaponHitbox.DisableHitbox();
            _enemy.PlayStaggerAnimation();
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _enemy.StaggerDuration)
                _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}