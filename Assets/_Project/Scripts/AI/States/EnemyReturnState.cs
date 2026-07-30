using SoulsLikeIsh.Core;
using SoulsLikeIsh.Character.Enemy;

namespace SoulsLikeIsh.AI
{
    public class EnemyReturnState : IState
    {
        private readonly EnemyController _enemy;

        public EnemyReturnState(EnemyController enemy) => _enemy = enemy;

        public void Enter()
        {
            _enemy.Agent.isStopped = false;
            _enemy.Agent.updateRotation = true;
            _enemy.Agent.SetDestination(_enemy.SpawnPoint);
        }

        public void Tick()
        {
            if (_enemy.Agent.pathPending) return;
            if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            {
                _enemy.Health.ResetToFull();
                _enemy.StateMachine.ChangeState(_enemy.IdleState);
            }
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}