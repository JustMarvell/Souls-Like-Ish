using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Enemy
{
    public class EnemyAttackState : IState
    {
        private readonly EnemyController _enemy;

        public EnemyAttackState(EnemyController enemy) => _enemy = enemy;

        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void FixedTick()
        {
            throw new System.NotImplementedException();
        }

        public void Tick()
        {
            throw new System.NotImplementedException();
        }
    }
}