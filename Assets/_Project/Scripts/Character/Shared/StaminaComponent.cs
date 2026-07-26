using UnityEngine;

namespace SoulsLikeIsh.Character.Shared
{
    public class StaminaComponent : MonoBehaviour
    {
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float regenRate = 25f;
        [SerializeField] private float regenDelay = 1f;

        public float Max => maxStamina;
        public float Current { get; private set; }

        private float _lastSpendTime = float.NegativeInfinity;

        private void Awake() => Current = maxStamina;

        public bool HasEnough(float amount) => Current >= amount;

        public bool TrySpend(float amount)
        {
            if (Current < amount) return false;
            Current -= amount;
            _lastSpendTime = Time.time;
            return true;
        }

        private void Update()
        {
            if (Current >= maxStamina) return;
            if (Time.time - _lastSpendTime < regenDelay) return;
            Current = Mathf.Min(maxStamina, Current + regenRate * Time.deltaTime);
        }
    }
}