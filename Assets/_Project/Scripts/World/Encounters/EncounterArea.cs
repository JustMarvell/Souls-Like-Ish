using System.Collections.Generic;
using UnityEngine;
using SoulsLikeIsh.Character.Enemy;

namespace SoulsLikeIsh.World.Encounters
{
    [RequireComponent(typeof(Collider))]
    public class EncounterArea : MonoBehaviour
    {
        [SerializeField] private List<EnemyController> enemies = new();
        [SerializeField] private EncounterChest chest;

        private int _aliveCount;

        private void Awake()
        {
            _aliveCount = enemies.Count;
            foreach (var enemy in enemies)
            {
                enemy.Health.OnDeath += HandleEnemyDeath;
                enemy.Encounter = this;
            }
        }

        private void OnDestroy()
        {
            foreach (var enemy in enemies)
                if (enemy != null) enemy.Health.OnDeath -= HandleEnemyDeath;
        }

        private void HandleEnemyDeath()
        {
            _aliveCount--;
            if (_aliveCount <= 0 && chest != null)
                chest.Unlock();
        }

        public void AlertGroup(Transform player)
        {
            foreach (var enemy in enemies)
                if (!enemy.Health.IsDead) enemy.AlertToChase(player);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            foreach (var enemy in enemies)
                if (!enemy.Health.IsDead) enemy.LeashBack();
        }
    }
}