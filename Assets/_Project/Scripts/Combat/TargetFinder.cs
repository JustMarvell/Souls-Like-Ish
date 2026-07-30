using System.Collections.Generic;
using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    public static class TargetFinder
    {
        public static ILockOnTarget FindBestLockOnTarget(Vector3 origin, Vector3 forward, float radius, float maxAngle, LayerMask mask)
        {
            var hits = Physics.OverlapSphere(origin, radius, mask);
            ILockOnTarget best = null;
            float bestAngle = maxAngle;

            foreach (var hit in hits)
            {
                var candidate = hit.GetComponentInParent<ILockOnTarget>();
                if (candidate == null || !candidate.IsTargetable) continue;

                Vector3 dir = candidate.LockOnPoint.position - origin;
                dir.y = 0f;
                if (dir.sqrMagnitude < 0.01f) continue;

                float angle = Vector3.Angle(forward, dir);
                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    best = candidate;
                }
            }
            return best;
        }

        public static ILockOnTarget FindNextLockOnTarget(Vector3 origin, ILockOnTarget current, float radius, LayerMask mask)
        {
            var candidates = FindLockOnTargets(origin, radius, mask);
            candidates.RemoveAll(t => t == current);
            if (candidates.Count == 0) return null;
            if (current == null) return candidates[0];

            Vector3 currentDir = current.LockOnPoint.position - origin;
            currentDir.y = 0f;
            float currentAngle = Mathf.Atan2(currentDir.x, currentDir.z) * Mathf.Rad2Deg;

            ILockOnTarget best = null;
            float bestDelta = 360f;

            foreach (var t in candidates)
            {
                Vector3 dir = t.LockOnPoint.position - origin;
                dir.y = 0f;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                float delta = Mathf.DeltaAngle(currentAngle, angle);
                if (delta <= 0f) delta += 360f;

                if (delta < bestDelta)
                {
                    bestDelta = delta;
                    best = t;
                }
            }
            return best ?? candidates[0];
        }

        public static List<ILockOnTarget> FindLockOnTargets(Vector3 origin, float radius, LayerMask mask)
        {
            var hits = Physics.OverlapSphere(origin, radius, mask);
            var targets = new List<ILockOnTarget>();

            foreach (var hit in hits)
            {
                var target = hit.GetComponentInParent<ILockOnTarget>();
                if (target != null && target.IsTargetable && !targets.Contains(target))
                    targets.Add(target);
            }
            return targets;
        }

        public static Transform FindNearestTarget(Vector3 origin, float radius, LayerMask mask)
        {
            var hits = Physics.OverlapSphere(origin, radius, mask);
            Transform best = null;
            float bestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                var target = hit.GetComponentInParent<ILockOnTarget>();
                if (target == null || !target.IsTargetable) continue;

                float dist = Vector3.Distance(origin, target.LockOnPoint.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = target.LockOnPoint;
                }
            }
            return best;
        }
    }
}