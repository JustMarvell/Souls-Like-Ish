using UnityEngine;
using SoulsLikeIsh.Character.Shared;

namespace SoulsLikeIsh.UI
{
    public class EnemyWorldBar : MonoBehaviour
    {
        [SerializeField] private GameObject visualsRoot;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private StatBar healthBar;
        [SerializeField] private StatBar staminaBar;
        [SerializeField] private float visibleDuration = 4f;
        [SerializeField] private float fadeSpeed = 4f;

        private HealthComponent _health;
        private StaminaComponent _stamina;
        private Camera _cam;
        private float _visibleTimer;

        private void Awake()
        {
            _health = GetComponentInParent<HealthComponent>();
            _stamina = GetComponentInParent<StaminaComponent>();
            _cam = Camera.main;
            visualsRoot.SetActive(false);
        }

        private void OnEnable() => _health.OnDamaged += HandleDamaged;
        private void OnDisable() => _health.OnDamaged -= HandleDamaged;

        private void HandleDamaged(int amount) => _visibleTimer = visibleDuration;

        private void Update()
        {
            _visibleTimer -= Time.deltaTime;
            bool shouldShow = _visibleTimer > 0f && !_health.IsDead;

            if (!shouldShow && canvasGroup.alpha <= 0f)
            {
                if (visualsRoot.activeSelf) visualsRoot.SetActive(false);
                return;
            }

            if (!visualsRoot.activeSelf) visualsRoot.SetActive(true);

            healthBar.SetValue(_health.Current, _health.Max);
            if (_stamina != null) staminaBar.SetValue(_stamina.Current, _stamina.Max);

            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, shouldShow ? 1f : 0f, fadeSpeed * Time.deltaTime);
            transform.rotation = _cam.transform.rotation;
        }
    }
}