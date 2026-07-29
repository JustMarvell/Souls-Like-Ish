using UnityEngine;
using Unity.Cinemachine;

namespace SoulsLikeIsh.CameraSystem
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraShaker : MonoBehaviour
    {
        public static CameraShaker Instance { get; private set; }

        private CinemachineImpulseSource _impulseSource;

        private void Awake()
        {
            Instance = this;
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void Shake(float force = 1f) => _impulseSource.GenerateImpulseWithForce(force);
        public void Shake(Vector3 velocity) => _impulseSource.GenerateImpulse(velocity);
    }
}