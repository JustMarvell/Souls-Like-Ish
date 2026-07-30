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
        [SerializeField] private int staggerPower = 20;
        [SerializeField] private StaggerType staggerType = StaggerType.Small;
        [SerializeField] private bool guaranteedStagger = false;

        public float Duration => duration;
        public float ActiveStart => activeStart;
        public float ActiveEnd => activeEnd;
        public int Damage => damage;
        public float StaminaCost => staminaCost;
        public int StaggerPower => staggerPower;
        public StaggerType StaggerType => staggerType;
        public bool GuaranteedStagger => guaranteedStagger;

        [SerializeField] private string animationState = "Attack";
        [SerializeField] private AttackData nextCombo;
        [SerializeField] private float comboWindowStart = 0.4f;
        [SerializeField] private float comboWindowEnd = 0.7f;
        [SerializeField] private bool canWarp = true;
        public bool CanWarp => canWarp;

        public string AnimationState => animationState;
        public AttackData NextCombo => nextCombo;
        public float ComboWindowStart => comboWindowStart;
        public float ComboWindowEnd => comboWindowEnd;
    }
}