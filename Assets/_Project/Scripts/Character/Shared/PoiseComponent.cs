using UnityEngine;

namespace SoulsLikeIsh.Character.Shared
{
    public class PoiseComponent : MonoBehaviour
    {
        [SerializeField] private float maxPoise = 40f;
        [SerializeField] private float regenRate = 30f;
        [SerializeField] private float regenDelay = 2f;

        public float Max => maxPoise;
        public float Current { get; private set; }

        private float _lastHitTime = float.NegativeInfinity;

        private void Awake() => Current = maxPoise;

        // Returns true if this hit broke poise (should interrupt).
        public bool ApplyStagger(float amount)
        {
            Current -= amount;
            _lastHitTime = Time.time;
            if (Current > 0f) return false;
            Current = maxPoise;
            return true;
        }

        private void Update()
        {
            if (Current >= maxPoise) return;
            if (Time.time - _lastHitTime < regenDelay) return;
            Current = Mathf.Min(maxPoise, Current + regenRate * Time.deltaTime);
        }
    }
}