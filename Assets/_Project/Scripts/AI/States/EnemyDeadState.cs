using SoulsLikeIsh.Core;
using SoulsLikeIsh.Character.Enemy;

namespace SoulsLikeIsh.AI
{
    public class EnemyDeadState : IState
    {
        private readonly EnemyController _enemy;

        public EnemyDeadState(EnemyController enemy) => _enemy = enemy;

        public void Enter()
        {
            _enemy.Agent.isStopped = true;
            _enemy.Agent.enabled = false;
            _enemy.WeaponHitbox.DisableHitbox();
            _enemy.PlayDeathAnimation();
            _enemy.Collider.enabled = false;
            _enemy.DestroyEnemy(5f);
            // TODO: disable hurtbox/colliders, drop loot, despawn/respawn once those systems exist.
        }

        public void Tick() { }
        public void FixedTick() { }
        public void Exit() { }
    }
}