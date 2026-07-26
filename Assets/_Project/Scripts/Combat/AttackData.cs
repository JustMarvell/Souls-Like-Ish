using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "SoulsLikeIsh/Combat/Attack Data")]
    public class AttackData : ScriptableObject
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float activeStart = 0.15f;
        [SerializeField] private float activeEnd = 0.3f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float staminaCost = 20f;

        public float Duration => duration;
        public float ActiveStart => activeStart;
        public float ActiveEnd => activeEnd;
        public int Damage => damage;
        public float StaminaCost => staminaCost;
    }
}