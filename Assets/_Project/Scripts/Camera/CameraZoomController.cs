using UnityEngine;
using Unity.Cinemachine;
using SoulsLikeIsh.Input;

namespace SoulsLikeIsh.CameraSystem
{
    public class CameraZoomController : MonoBehaviour
    {
        [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private float minRadius = 2f;
        [SerializeField] private float maxRadius = 8f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float smoothTime = 0.15f;

        private float _targetRadius;
        private float _velocity;

        private void Start() => _targetRadius = orbitalFollow.Radius;

        private void Update()
        {
            _targetRadius = Mathf.Clamp(_targetRadius - inputReader.ZoomInput * zoomSpeed, minRadius, maxRadius);
            orbitalFollow.Radius = Mathf.SmoothDamp(orbitalFollow.Radius, _targetRadius, ref _velocity, smoothTime);
        }
    }
}