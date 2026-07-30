using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    public readonly struct DamageInfo
    {
        public readonly int Amount;
        public readonly int StaggerPower;
        public readonly GameObject Source;

        public DamageInfo(int amount, int staggerPower, GameObject source)
        {
            Amount = amount;
            StaggerPower = staggerPower;
            Source = source;
        }
    }
}