using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffSlotHome : MonoBehaviour, IInteractable
{
    private BuffItemPro buff;
    [SerializeField] private TextMeshPro buffNameText;
    [SerializeField] private TextMeshPro buffPriceText;
    private PlayerWallet wallet;

    void Start()
    {
        wallet = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWallet>();
        // Please remember to set the buff layer to 'Buff'
    }

    void Update()
    {

    }

    public void GetBuff(BuffItemPro newBuff)
    {
        buff = newBuff;
        DisplayBuffInfo();
    }

    private void DisplayBuffInfo()
    {
        DestroyBuffChildren();
        GameObject fakeBuff = Instantiate(buff.buffModel, transform.position, Quaternion.identity);
        fakeBuff.transform.SetParent(transform, true);

        buffNameText.text = buff.buffName;
        buffPriceText.text = buff.buffCreditCurrencyCost.ToString();
    }

    public void OnInteract()
    {
        Transform[] slots = HomeMerchantPro.Instance.ItemSpawnSlotArray;
        int price = buff.buffCreditCurrencyCost;
        if (HomeMerchantPro.Instance.RemainingBuyTurns > 0)
        {
            if (wallet.DeductCredit(price) == true)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    // Check if the slot doesn't have any child
                    if ((slots[i].childCount == 0))
                    {
                        HomeMerchantPro.Instance.ModifyRemainingBuyTurns(-1);
                        DestroyBuffChildren();

                        GameObject realBuff = Instantiate(buff.buffPrefab, slots[i].transform.position, Quaternion.identity);
                        realBuff.transform.SetParent(slots[i].transform, true);
                        return;
                    }
                }
                Debug.LogWarning("No empty slots available to spawn the buff.");
            }
            else
            {
                Debug.Log("Not enough bio compound to purchase the buff.");
            }
        }
        else
        {
            Debug.Log("No more buy turns left.");
        }
    }

    private void DestroyBuffChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.layer == LayerMask.NameToLayer("Buff"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
