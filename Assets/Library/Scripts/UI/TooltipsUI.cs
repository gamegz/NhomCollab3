using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class TooltipsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI barricadeText; 
    [SerializeField] private TextMeshProUGUI OverHealText;
    [SerializeField] private TextMeshProUGUI HealText;
    [SerializeField] private float fadeDuration = 1f;
    private BoxCollider boxCollider;
    [SerializeField] private string barricadeInstruction;
    [SerializeField] private string overHealInstruction;
    [SerializeField] private string healInstruction;

    private void Start()
    {
        Color color = barricadeText.color;
        Color color1 = OverHealText.color;
        Color color2 = HealText.color;
        color.a = 0f;
        color1.a = 0f;
        color2.a = 0f;
        barricadeText.color = color;
        OverHealText.color = color1;
        HealText.color = color2;
        barricadeText.gameObject.SetActive(false);
        OverHealText.gameObject.SetActive(false);
        HealText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        barricadeText.DOKill();
        OverHealText.DOKill();
        HealText.DOKill();
    }

    private void OnDisable()
    {
        barricadeText.DOKill();
        OverHealText.DOKill();
        HealText.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InstructionCollider"))
        {
            barricadeText.gameObject.SetActive(true);
            barricadeText.text = barricadeInstruction;
            barricadeText.DOFade(1f, fadeDuration);
            DOVirtual.DelayedCall(2f, () => barricadeText.DOFade(0f, fadeDuration).OnComplete(() => barricadeText.gameObject.SetActive(false)));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InstructionCollider"))
        {
            barricadeText.DOFade(0f, fadeDuration).OnComplete((() => { barricadeText.gameObject.SetActive(false); }));
            other.GetComponent<BoxCollider>().enabled = false;
            OverHealText.text = overHealInstruction;
            OverHealText.gameObject.SetActive(true);
            DOVirtual.DelayedCall(3f, () => OverHealText.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                DOVirtual.DelayedCall(2f,
                    () => OverHealText.DOFade(0f, fadeDuration).OnComplete(() =>
                    {
                        OverHealText.gameObject.SetActive(false);
                    }));
            }));
        }
    }

    public void HealInstruction()
    {
        HealText.text = healInstruction;
        HealText.gameObject.SetActive(true);
        HealText.DOFade(1f, fadeDuration);
        DOVirtual.DelayedCall(2f, () => HealText.DOFade(0f, fadeDuration).OnComplete(() => HealText.gameObject.SetActive(false)));
    }
}
