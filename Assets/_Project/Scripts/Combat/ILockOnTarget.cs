using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    public interface ILockOnTarget
    {
        Transform LockOnPoint { get; }
        bool IsTargetable { get; }
    }
}