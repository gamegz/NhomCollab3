using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffSlot : MonoBehaviour, IInteractable
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
        GameObject fakeBuff = Instantiate(buff.itemModel, transform.position, Quaternion.identity);
        fakeBuff.transform.SetParent(transform, true);

        buffNameText.text = buff.itemName;
        buffPriceText.text = buff.itemBioCost.ToString();
    }

    /*
        1. Check if the player has remaining purchase turns
        2. Check if the player has enough money 
        3. Select the slot to spawn the item
    */
    public void OnInteract()
    {
        Transform[] slots = LevelMerchantPro.Instance.ItemSpawnSlotArray;
        int price = buff.itemBioCost;
        if (LevelMerchantPro.Instance.remainingBuyTurns > 0)
        {
            if (PlayerWallet.P_WalletInstance.DeductBioCompound(price) == true)
            {

                for (int i = 0; i < slots.Length; i++)
                {
                    // Check if the slot doesn't have any child
                    if ((slots[i].childCount == 0))
                    {
                        LevelMerchantPro.Instance.ModifyRemainingRerolls(0);
                        LevelMerchantPro.Instance.ModifyRemainingBuyTurns(-1);
                        LevelMerchantPro.Instance.UpdateRerollInfo();
                        DestroyBuffChildren();

                        GameObject realBuff = Instantiate(buff.itemPrefab, slots[i].transform.position, Quaternion.identity);
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

    // Remove any existing weapon objects from freeWeaponSlot before spawning a new weapon.
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
