using System;
using UnityEngine;

namespace SoulsLikeIsh.Character.Shared
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        public int Max => maxHealth;
        public int Current { get; private set; }
        public bool IsDead => Current <= 0;

        public event Action<int> OnDamaged;
        public event Action OnDeath;

        private void Awake() => Current = maxHealth;

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;

            Current = Mathf.Max(0, Current - amount);
            OnDamaged?.Invoke(amount);

            if (IsDead) OnDeath?.Invoke();
        }
    }
}