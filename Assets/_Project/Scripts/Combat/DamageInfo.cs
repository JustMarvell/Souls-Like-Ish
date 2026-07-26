using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    public readonly struct DamageInfo
    {
        public readonly int Amount;
        public readonly GameObject Source;

        public DamageInfo(int amount, GameObject source)
        {
            Amount = amount;
            Source = source;
        }
    }
}