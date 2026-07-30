using UnityEngine;
using UnityEngine.UI;

namespace SoulsLikeIsh.UI
{
    [RequireComponent(typeof(Slider))]
    public class StatBar : MonoBehaviour
    {
        [SerializeField] private float smoothSpeed = 10f;
        private Slider _slider;

        private void Awake() => _slider = GetComponent<Slider>();

        public void SetValue(float current, float max)
        {
            float target = max > 0f ? current / max : 0f;
            _slider.value = Mathf.MoveTowards(_slider.value, target, smoothSpeed * Time.deltaTime);
        }
    }
}