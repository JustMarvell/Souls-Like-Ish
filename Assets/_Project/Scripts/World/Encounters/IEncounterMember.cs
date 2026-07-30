using UnityEngine;

namespace SoulsLikeIsh.World.Encounters
{
    public interface IEncounterMember
    {
        bool IsDead { get; }
        void AlertToChase(Transform player);
        void LeashBack();
    }
}