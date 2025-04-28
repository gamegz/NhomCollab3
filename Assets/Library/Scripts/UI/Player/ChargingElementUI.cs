using Core.Events;
using DG.Tweening;
using Library.Scripts.Player.Other;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace Library.Scripts.UI.Player
{
    // ps dont do this this is product of laziness just put it all in a list or something then loop
    public class ChargingElementUI : MonoBehaviour
    {
        public RectTransform rootTransform;
        public RectTransform container;

        [Space] 
        public Slider attackChargeSlider;
        public Slider cooldownChargeSlider;

        [Space] 
        public CanvasGroup attackChargeCanvasGroup;
        public CanvasGroup cooldownChargeCanvasGroup;

        private PlayerWorldPositionReference _positionReference;
        private Camera _camera;

        private bool _attackChargeState;
        private bool _isAttackChargeFilled;
        private bool _cooldownChargeState;

        private Tween _attackChargeTween;
        private Sequence _attackChargeFlash;
        private Color _originalAttackChargeColor;
        private Image _attackChargeImageFill;
        
        private Tween _coolDownChargeTween;
        
        private void Awake()
        {
            this.AddListener(EventType.UISendPositionReference, HandleChargePosition);
            this.AddListener(EventType.UIOnAttackCooldown, HandleAttackCooldown);
            this.AddListener(EventType.UIOnAttackCharge, HandleAttackCharge);

            attackChargeCanvasGroup.alpha = 0;
            cooldownChargeCanvasGroup.alpha = 0;

            _attackChargeImageFill = attackChargeSlider.fillRect.GetComponent<Image>();
            _originalAttackChargeColor = _attackChargeImageFill.color;
        }

        private void OnDestroy()
        {
            this.RemoveListener(EventType.UISendPositionReference, HandleChargePosition);
            this.RemoveListener(EventType.UIOnAttackCooldown, HandleAttackCooldown);
            this.RemoveListener(EventType.UIOnAttackCharge, HandleAttackCharge);
        }

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnload;
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnload;
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        private void HandleAttackCharge(object obj)
        {
            if (obj is not UIChargingData chargingData || !attackChargeSlider) return;

            if (_attackChargeState != chargingData.state)
            {
                _attackChargeState = chargingData.state;
                _attackChargeTween?.Kill();
                _attackChargeTween = attackChargeCanvasGroup.DOFade(chargingData.state ? 1 : 0, 0.2f).OnComplete(() =>
                {
                    _attackChargeImageFill.color = _originalAttackChargeColor;
                }).OnKill(() =>
                {
                    _attackChargeImageFill.color = _originalAttackChargeColor;
                });
                _isAttackChargeFilled = false;
            }
            
            if (chargingData.maxValue == 0)
            {
                attackChargeSlider.value = 0;
                return;
            }
            attackChargeSlider.value = chargingData.currentValue / chargingData.maxValue;

            if (attackChargeSlider.value >= 1 && !_isAttackChargeFilled)
            {
                _isAttackChargeFilled = true;
                
                _attackChargeFlash?.Kill();
                _attackChargeFlash = DOTween.Sequence();

                _attackChargeFlash
                    .Append(_attackChargeImageFill.DOColor(Color.white, 0.2f))
                    .Append(_attackChargeImageFill.DOColor(_originalAttackChargeColor, 0.2f));
            }
        }

        private void HandleAttackCooldown(object obj)
        {
            if (obj is not UIChargingData chargingData || !cooldownChargeSlider) return;
            
            if (_cooldownChargeState != chargingData.state)
            {
                _cooldownChargeState = chargingData.state;
                _coolDownChargeTween?.Kill();
                _coolDownChargeTween = cooldownChargeCanvasGroup.DOFade(chargingData.state ? 1 : 0, 0.2f);
            }
            
            if (chargingData.maxValue == 0)
            {
                cooldownChargeSlider.value = 0;
                return;
            }
            cooldownChargeSlider.value = chargingData.currentValue / chargingData.maxValue;
        }

        private void HandleChargePosition(object obj)
        {
            if (obj is PlayerWorldPositionReference positionReference)
            {
                _positionReference = positionReference;
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            if (!_positionReference) return;
            if (!_camera) return;
            
            var screenPoint = _camera.WorldToScreenPoint(_positionReference.toGetPosition);
                
            // Applicable with canvas of type Screen Space - Overlay only, else it'll need a camera btw.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootTransform,
                screenPoint,
                null,
                out var anchoredPosition);

            container.anchoredPosition = anchoredPosition;
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            this.AddListener(EventType.UISendPositionReference, HandleChargePosition);
            this.AddListener(EventType.UIOnAttackCooldown, HandleAttackCooldown);
            this.AddListener(EventType.UIOnAttackCharge, HandleAttackCharge);

            attackChargeCanvasGroup.alpha = 0;
            cooldownChargeCanvasGroup.alpha = 0;

            _attackChargeImageFill = attackChargeSlider.fillRect.GetComponent<Image>();
            _originalAttackChargeColor = _attackChargeImageFill.color;
        }

        private void OnSceneUnload(Scene scene)
        {
            this.RemoveListener(EventType.UISendPositionReference, HandleChargePosition);
            this.RemoveListener(EventType.UIOnAttackCooldown, HandleAttackCooldown);
            this.RemoveListener(EventType.UIOnAttackCharge, HandleAttackCharge);
        }
    }
}