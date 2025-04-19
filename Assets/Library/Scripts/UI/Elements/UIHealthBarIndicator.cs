using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Library.Scripts.UI.Elements
{
    public class UIHealthBarIndicator : MonoBehaviour
    {
        public enum HealthBarState
        {
            Idle,
            Disabled,
            Enabled,
        }
        
        public Image healthBarImage;

        [Header("Config")] 
        public float heartBeatScaleExpand = 1.1f;
        public float heartBeatScaleDuration = 0.2f;
        public float heartBeatScaleDelayPerBeat = 0.2f;
        [Space] 
        public float heartBeatDisableExpand = 1.1f;
        public float heartBeatDisableExpandDuration = 0.2f;
        public float heartBeatDisableShrinkDuration = 0.2f;
        [Space]
        public float heartBeatEnableDuration = 0.2f;

        private Sequence _sequence;
        public bool isActive { get; private set; }

        public void Enable(bool useTween)
        {
            SwitchState(HealthBarState.Enabled, useTween);
        }

        public void Disable(bool useTween)
        {
            SwitchState(HealthBarState.Disabled, useTween);
        }
        
        private void SwitchState(HealthBarState state, bool useTween)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            switch (state)
            {
                case HealthBarState.Idle:
                    transform.localScale = Vector3.one;
                    _sequence
                        .Append(transform.DOScale(heartBeatScaleExpand, heartBeatScaleDuration).SetEase(Ease.OutBack))
                        .Append(transform.DOScale(Vector3.one, heartBeatScaleDuration).SetEase(Ease.OutBack))
                        .Append(DOVirtual.DelayedCall(heartBeatScaleDelayPerBeat, () => { }))
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                case HealthBarState.Disabled:
                    isActive = false;
                    if (!useTween)
                    {
                        transform.localScale = Vector3.zero;
                        return;
                    }
                    transform.localScale = Vector3.one;
                    _sequence
                        .Append(transform.DOScale(heartBeatDisableExpand, heartBeatDisableExpandDuration)
                            .SetEase(Ease.OutBack))
                        .Append(transform.DOScale(Vector3.zero, heartBeatDisableShrinkDuration));
                        
                    break;
                case HealthBarState.Enabled:
                    isActive = true;
                    if (!useTween)
                    {
                        transform.localScale = Vector3.one;
                        return;
                    }
                    transform.localScale = Vector3.zero;
                    _sequence
                        .Append(transform.DOScale(Vector3.one, heartBeatEnableDuration).SetEase(Ease.OutExpo))
                        .AppendCallback(() =>
                        {
                            SwitchState(HealthBarState.Idle, true);
                        });
                    break;
            }
        }
    }
}