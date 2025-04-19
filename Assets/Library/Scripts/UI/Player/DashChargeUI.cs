using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Library.Scripts.UI.Player
{
    public class DashChargeUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private Slider dashSlider;
        [SerializeField] private Transform dashSliderContainer;
        [SerializeField] private GameObject blackBar;

        [Header("Visuals")]
        public Image fillImage;
        public Color flashColor = Color.white;
        public float flashTime = 0.2f;
        public float shakeStrength = 5f;
        public int shakeVibration = 25;

        private Sequence _flashTween;
        private Color _originalIndicatorColor;

        private IEnumerator Start()
        {
            if (playerMovement == null) yield break;
            
            PlayerMovement.dashIndicate += FlashIndicator;
            _originalIndicatorColor = fillImage.color;
            
            var dashAmount = playerMovement.GetMaxCharge();
            if (dashAmount <= 1) yield break;
            if (dashAmount <= 2)
            {
                var inst = Instantiate(blackBar, dashSliderContainer);
                inst.transform.localScale = Vector3.one;
                inst.transform.localPosition = Vector3.zero;
                inst.SetActive(true);
                yield break;
            }
            
            yield return new WaitForEndOfFrame();
            
            var containerLength = ((RectTransform)dashSliderContainer).rect.width;
            var localXPerBar = containerLength / dashAmount;
            var currenOffset = -(containerLength / 2) + localXPerBar;
            
            for (int i = 0; i < dashAmount - 1; i++)
            {
                var inst = Instantiate(blackBar, dashSliderContainer);
                inst.transform.localScale = Vector3.one;
                inst.SetActive(true);
                
                inst.transform.localPosition = new Vector3(currenOffset, 0, 0);
                currenOffset += localXPerBar;
            }
        }

        private void OnDestroy()
        {
            PlayerMovement.dashIndicate -= FlashIndicator;
        }

        private void FlashIndicator()
        {
            _flashTween?.Kill();
            _flashTween = DOTween.Sequence();
            _flashTween
                .Append(fillImage.DOColor(flashColor, flashTime))
                .Insert(0, dashSlider.transform.DOShakeRotation(flashTime, new Vector3(0, 0, shakeStrength), shakeVibration))
                .Append(DOVirtual.DelayedCall(0.1f, () => { }))
                .Append(fillImage.DOColor(_originalIndicatorColor, flashTime * 2));
        }

        private void Update()
        {
            if (!playerMovement) return;
            var ratio = (playerMovement.CurrentCharge * playerMovement.DashRecoverTimePerCharge + playerMovement.DashRecoverTimePerChargeCount) / 
                        playerMovement.totalDashTime;
            dashSlider.value = ratio;
        }
    }
}
