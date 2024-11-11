using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponSlot : MonoBehaviour, IInteractable
{
    private WeaponItemPro weapon;
    [SerializeField] private TextMeshPro weaponNameText;
    [SerializeField] private TextMeshPro weaponPriceText;
    private PlayerWallet wallet;




    void Start()
    {
        wallet = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWallet>();
        Debug.LogWarning("Please remember to set the weapon layer to 'Weapon'!!!");
    }

    void Update()
    {
        if (wallet == null)
        {
            Debug.Log("WeaponSlot tried to find a gameObject with the tag 'Player' but it seems you moron forgot to add it!");
        }
    }

    public void GetWeapon(WeaponItemPro newWeapon)
    {
        weapon = newWeapon;
        DisplayeWeaponInfo();
    }

    private void DisplayeWeaponInfo()
    {
        DestroyWeaponChildren();
        GameObject fakeWeapon = Instantiate(weapon.weaponModel, transform.position, Quaternion.identity);
        fakeWeapon.transform.SetParent(transform, true);

        weaponNameText.text = weapon.weaponName;
        weaponPriceText.text = weapon.weaponBioCurrencyCost.ToString();
    }


    public void OnInteract()
    {
        Transform[] slots = LevelMerchantPro.Instance.ItemSpawnSlotArray;
        int price = weapon.weaponBioCurrencyCost;
        if (wallet.DeductBioCompound(price) == true)
        {
            if (LevelMerchantPro.Instance.RemainingBuyTurns > 0)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    // Check if the slot doesn't have any child
                    if ((slots[i].childCount == 0))
                    {
                        LevelMerchantPro.Instance.remainingRerolls = 0;


                        LevelMerchantPro.Instance.ModifyRemainingBuyTurns(-1);



                        LevelMerchantPro.Instance.UpdateRerollInfo();
                        DestroyWeaponChildren();
                        GameObject realWeapon = Instantiate(weapon.weaponPrefab, slots[i].transform.position, Quaternion.identity);
                        realWeapon.transform.SetParent(slots[i].transform, true);
                        Debug.Log("Weapon spawned in slot: " + i);
                        return;
                    }
                }
                Debug.Log("No empty slots available to spawn the weapon.");

            }
            else
            {
                Debug.Log("No more turn");
            }
        }
    }

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
