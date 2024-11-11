using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffSlot : MonoBehaviour, IInteractable
{
    private BuffItemPro buff;
    [SerializeField] private TextMeshPro buffNameText;
    [SerializeField] private TextMeshPro buffPriceText;
    private PlayerWallet wallet;

    void Start()
    {
        wallet = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWallet>();
        Debug.LogWarning("Please remember to set the buff layer to 'Buff'!!!");
    }

    void Update()
    {
        if (wallet == null)
        {
            Debug.Log("BuffSlot tried to find a gameObject with the tag 'Player' but it seems you moron forgot to add it!");
        }
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
        buffPriceText.text = buff.buffBioCurrencyCost.ToString();
    }

    public void OnInteract()
    {
        Transform[] slots = LevelMerchantPro.Instance.ItemSpawnSlotArray;
        int price = buff.buffBioCurrencyCost;
        if (LevelMerchantPro.Instance.RemainingBuyTurns > 0)
        {
            if (wallet.DeductBioCompound(price) == true)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if ((slots[i].childCount == 0))
                    {
                        LevelMerchantPro.Instance.remainingRerolls = 0;
                        LevelMerchantPro.Instance.ModifyRemainingBuyTurns(-1);
                        LevelMerchantPro.Instance.UpdateRerollInfo();
                        DestroyBuffChildren();
                        GameObject realBuff = Instantiate(buff.buffPrefab, slots[i].transform.position, Quaternion.identity);
                        realBuff.transform.SetParent(slots[i].transform, true);
                        Debug.Log("Weapon spawned in slot: " + i);
                        return;
                    }
                }
                Debug.Log("No empty slots available to spawn the buff.");
            }
        }
        else
        {
            Debug.Log("No more turn");
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
