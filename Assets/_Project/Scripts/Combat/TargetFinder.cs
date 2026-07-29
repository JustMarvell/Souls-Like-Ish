using UnityEngine;

namespace SoulsLikeIsh.Combat
{
    public static class TargetFinder
    {
        public static Transform FindBestTarget(Vector3 origin, Vector3 forward, float radius, float maxAngle, LayerMask mask)
        {
            var hits = Physics.OverlapSphere(origin, radius, mask);
            Transform best = null;
            float bestAngle = maxAngle;

            foreach (var hit in hits)
            {
                Vector3 dir = hit.transform.position - origin;
                dir.y = 0f;
                if (dir.sqrMagnitude < 0.01f) continue;

                float angle = Vector3.Angle(forward, dir);
                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    best = hit.transform;
                }
            }
            return best;
        }
    }
}