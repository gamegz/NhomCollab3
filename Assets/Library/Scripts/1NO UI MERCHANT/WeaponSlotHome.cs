using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponSlotHome : MonoBehaviour, IInteractable
{
    private WeaponItemPro weapon;
    [SerializeField] private TextMeshPro weaponNameText;
    [SerializeField] private TextMeshPro weaponPriceText;
    private PlayerWallet wallet;

    void Start()
    {
        wallet = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWallet>();
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
        GameObject fakeWeapon = Instantiate(weapon.weaponModel, transform.position, Quaternion.identity);
        fakeWeapon.transform.SetParent(transform, true);

        weaponNameText.text = weapon.weaponName;
        weaponPriceText.text = weapon.weaponBioCurrencyCost.ToString();
    }

    public void OnInteract()
    {
        Transform[] slots = HomeMerchantPro.Instance.ItemSpawnSlotArray;
        int price = weapon.weaponCreditCurrencyCost;
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
                        DestroyWeaponChildren();

                        GameObject realWeapon = Instantiate(weapon.weaponPrefab, slots[i].transform.position, Quaternion.identity);
                        realWeapon.transform.SetParent(slots[i].transform, true);
                        Debug.Log("Weapon spawned in slot: " + i);
                        return;
                    }
                }
                Debug.LogWarning("No empty slots available to spawn the weapon.");
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
