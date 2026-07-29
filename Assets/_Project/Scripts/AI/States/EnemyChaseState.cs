using SoulsLikeIsh.Core;
using SoulsLikeIsh.Character.Enemy;

namespace SoulsLikeIsh.AI
{
    public class EnemyChaseState : IState
    {
        private readonly EnemyController _enemy;

        public EnemyChaseState(EnemyController enemy) => _enemy = enemy;

        public void Enter()
        {
            _enemy.Agent.isStopped = false;
            _enemy.Agent.updateRotation = true;
        }

        public void Tick()
        {
            if (!_enemy.CanSeePlayer())
            {
                _enemy.StateMachine.ChangeState(_enemy.IdleState);
                return;
            }

            if (_enemy.DistanceToPlayer() <= _enemy.AttackRange && _enemy.Stamina.HasEnough(_enemy.StaminaCostPerAttack))
            {
                _enemy.StateMachine.ChangeState(_enemy.AttackState);
                return;
            }

            _enemy.Agent.SetDestination(_enemy.Player.position);
        }

        public void FixedTick() { }
        public void Exit() { }
    }
}