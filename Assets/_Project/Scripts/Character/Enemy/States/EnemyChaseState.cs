using SoulsLikeIsh.Core;

namespace SoulsLikeIsh.Character.Enemy
{
    public class EnemyChaseState : IState
    {
        private readonly EnemyController _enemy;

        public EnemyChaseState(EnemyController enemy) => _enemy = enemy;

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