using UnityEngine;
using Unity.Cinemachine;
using SoulsLikeIsh.Combat;
using SoulsLikeIsh.Input;

namespace SoulsLikeIsh.CameraSystem
{
    public class LockOnController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform player;
        [SerializeField] private LayerMask targetableLayers;
        [SerializeField] private float searchRadius = 12f;
        [SerializeField] private float maxLockAngle = 60f;
        [SerializeField] private float breakDistance = 15f;

        public bool IsLocked { get; private set; }
        public ILockOnTarget CurrentTarget { get; private set; }

        private Camera _cam;
        private Transform defaultLookAtTarget;

        private void Awake()
        {
            _cam = Camera.main;
            defaultLookAtTarget = cinemachineCamera.Target.LookAtTarget;
        }

        private void OnEnable() => inputReader.OnLockOn += ToggleLock;
        private void OnDisable() => inputReader.OnLockOn -= ToggleLock;

        private void Update()
        {
            if (!IsLocked) return;

            if (!CurrentTarget.IsTargetable ||
                Vector3.Distance(player.position, CurrentTarget.LockOnPoint.position) > breakDistance)
            {
                ClearTarget();
            }
        }

        private void ToggleLock()
        {
            if (IsLocked) { ClearTarget(); return; }

            var target = FindClosestToScreenCenter();
            if (target != null) SetTarget(target);
        }

        private ILockOnTarget FindClosestToScreenCenter()
        {
            var candidates = TargetFinder.FindLockOnTargets(player.position, searchRadius, targetableLayers);
            ILockOnTarget best = null;
            float bestScore = float.MaxValue;

            foreach (var candidate in candidates)
            {
                Vector3 dir = candidate.LockOnPoint.position - player.position;
                dir.y = 0f;
                if (Vector3.Angle(player.forward, dir) > maxLockAngle) continue;

                Vector3 viewportPos = _cam.WorldToViewportPoint(candidate.LockOnPoint.position);
                if (viewportPos.z < 0f) continue;

                float screenDist = Vector2.Distance(new Vector2(viewportPos.x, viewportPos.y), new Vector2(0.5f, 0.5f));
                if (screenDist < bestScore)
                {
                    bestScore = screenDist;
                    best = candidate;
                }
            }
            return best;
        }

        public void SwitchTarget(ILockOnTarget target)
        {
            if (target == null || target == CurrentTarget) return;
            SetTarget(target);
        }

        private void SetTarget(ILockOnTarget target)
        {
            CurrentTarget = target;
            IsLocked = true;
            cinemachineCamera.Target.LookAtTarget = target.LockOnPoint;
        }

        private void ClearTarget()
        {
            CurrentTarget = null;
            IsLocked = false;
            cinemachineCamera.Target.LookAtTarget = defaultLookAtTarget;
        }
    }
}