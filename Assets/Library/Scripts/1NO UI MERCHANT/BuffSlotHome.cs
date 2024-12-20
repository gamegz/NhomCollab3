using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffSlotHome : MonoBehaviour, IInteractable
{
    private BuffItemPro buff;
    [SerializeField] private TextMeshPro buffNameText;
    [SerializeField] private TextMeshPro buffPriceText;

    void Start()
    {
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
        GameObject fakeBuff = Instantiate(buff.itemPrefab, transform.position, Quaternion.identity);
        fakeBuff.transform.SetParent(transform, true);

        buffNameText.text = buff.itemName;
        buffPriceText.text = buff.itemCreditCost.ToString();
    }

    public void OnInteract()
    {
        Transform[] slots = HomeMerchantPro.Instance.itemSpawnSlotArray;
        int price = buff.itemCreditCost;
        if (HomeMerchantPro.Instance.remainingBuyTurns > 0)
        {
            if (PlayerWallet.P_WalletInstance.DeductCredit(price) == true)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    // Check if the slot doesn't have any child
                    if ((slots[i].childCount == 0))
                    {
                        HomeMerchantPro.Instance.ModifyRemainingBuyTurns(-1);
                        DestroyBuffChildren();

                        GameObject realBuff = Instantiate(buff.itemPrefab, slots[i].position, Quaternion.identity);
                        realBuff.transform.SetParent(slots[i], true);
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
