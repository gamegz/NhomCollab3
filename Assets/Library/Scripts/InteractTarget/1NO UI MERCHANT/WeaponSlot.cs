using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponSlot : MonoBehaviour, IInteractable
{
    private WeaponItemPro weapon;
    [SerializeField] private TextMeshPro weaponNameText;
    [SerializeField] private TextMeshPro weaponPriceText;

    void Start()
    {
        //Please remember to set the weapon layer to 'Weapon'
    }

    void Update()
    {

    }

    public void GetWeapon(WeaponItemPro newWeapon)
    {
        weapon = newWeapon;
        DisplayeWeaponInfo();
    }

    private void DisplayeWeaponInfo()
    {
        DestroyWeaponChildren();
        GameObject fakeWeapon = Instantiate(weapon.itemModel, transform.position, Quaternion.identity);
        fakeWeapon.transform.SetParent(transform, true);

        weaponNameText.text = weapon.itemName;
        weaponPriceText.text = weapon.itemBioCost.ToString();
    }


    /*
    1. Check if the player has remaining purchase turns
    2. Check if the player has enough money 
    3. Select the slot to spawn the item
*/
    public void OnInteract()
    {
        Transform[] slots = LevelMerchantPro.Instance.ItemSpawnSlotArray;
        int price = weapon.itemBioCost;
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
                        DestroyWeaponChildren();

                        GameObject realWeapon = Instantiate(weapon.itemPrefab, slots[i].position, Quaternion.identity);
                        realWeapon.transform.SetParent(slots[i], true);
                        Debug.Log("Weapon spawned in slot: " + i);
                        return;
                    }
                }
                Debug.LogWarning("No empty slots available to spawn the weapon.");
                // Drop at player location(Optional)
                //GameObject realWeapon = Instantiate(weapon.itemPrefab, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("Not enough bio compound to purchase the weapon.");
            }
        }
        else
        {
            Debug.Log("No more buy turns left.");
        }
    }

    // Remove any existing weapon objects from freeWeaponSlot before spawning a new weapon.
    private void DestroyWeaponChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
