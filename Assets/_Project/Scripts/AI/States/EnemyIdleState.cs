using SoulsLikeIsh.Core;
using SoulsLikeIsh.Character.Enemy;

namespace SoulsLikeIsh.AI
{
    public class EnemyIdleState : IState
    {
        private readonly EnemyController _enemy;

        public EnemyIdleState(EnemyController enemy) => _enemy = enemy;

        public void Enter()
        {
            _enemy.Agent.isStopped = true;
        }

        public void Tick()
        {
            if (!_enemy.CanSeePlayer()) return;

            if (_enemy.Encounter != null)
                _enemy.Encounter.AlertGroup(_enemy.Player);
            else
                _enemy.StateMachine.ChangeState(_enemy.ChaseState);
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}