using DG.Tweening;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

namespace Library.Scripts.UI.Player
{
    public class OverHealBarUI : MonoBehaviour
    {
        [SerializeField] private Slider overHealSlider;
        [SerializeField] private Image overHealFill;
        public float tweenTime = 0.1f;
        public Color lerpColor = Color.red;

        [Space] 
        public float shakeSpeed = 2f;
        public float shakeOffset = 2f;
        public float maxShake = 5f;
        
        private Tween _numberTween;
        private Color _originalColor;
        private float _currentRatio;
        
        private void Awake()
        {
            PlayerBase.OnOverHealValueChange += HandleOverHeal;
            
            _originalColor = overHealFill.color;
        }

        private void HandleOverHeal(float currentClampedValue, float maxvalue, bool shouldUpdateViaTween)
        {
            if (!overHealSlider) return;
            var ratio = currentClampedValue / maxvalue;
            _currentRatio = ratio;

            overHealFill.color = Color.Lerp(_originalColor, lerpColor, ratio);
            
            if (shouldUpdateViaTween)
            {
                _numberTween?.Kill();
                _numberTween = DOVirtual.Float(overHealSlider.value, ratio, tweenTime, value =>
                {
                    overHealSlider.value = value;
                });
                
                return;
            }
            
            overHealSlider.value = currentClampedValue / maxvalue;
        }

        private void Update()
        {
            if (_currentRatio <= 0)
            {
                overHealSlider.transform.localRotation = Quaternion.identity;
                return;
            }
        
            var time = Time.time * Mathf.Lerp(0, shakeSpeed, _currentRatio) + shakeOffset;
            var angle = Mathf.Sin(time) * maxShake * _currentRatio;
        
            overHealSlider.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, 0, angle);
        }
        
    }
}