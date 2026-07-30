using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    public readonly struct DamageInfo
    {
        public readonly int Amount;
        public readonly int StaggerPower;
        public readonly StaggerType StaggerType;
        public readonly GameObject Source;

        public DamageInfo(int amount, int staggerPower, StaggerType staggerType, GameObject source)
        {
            Amount = amount;
            StaggerPower = staggerPower;
            StaggerType = staggerType;
            Source = source;
        }
    }
}