using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class LevelShopUI : MonoBehaviour
{
    public static LevelShopUI Instance { get; private set; }

    [SerializeField] private Button closeLevelShopPanelButton;
    [SerializeField] private GameObject levelShopPanel;
    [SerializeField] private Transform[] itemSpawnSlotArray;
    [SerializeField] private PlayerWallet wallet;




    [Space(50)]
    [Header("-------------------- WEAPON --------------------")]
    [SerializeField] private List<WeaponItem> weaponItemList;
    private WeaponItem[] selectedWeapons = new WeaponItem[2];
    [SerializeField] private Transform[] weaponDisplaySlotArray;
    [SerializeField] private Button[] weaponPurchaseButtons;
    [SerializeField] private Image[] weaponIcons;
    [SerializeField] private TextMeshProUGUI[] weaponNameTexts;
    [SerializeField] private TextMeshProUGUI[] weaponPriceTexts;

    [Space(50)]
    [Header("-------------------- BUFF --------------------")]
    [SerializeField] private List<BuffItem> buffItems;
    private BuffItem[] selectedBuffs = new BuffItem[2];
    [SerializeField] private Transform[] buffDisplaySlotArray;
    [SerializeField] private Button[] buffPurchaseButtons;
    [SerializeField] private Image[] buffIcons;
    [SerializeField] private TextMeshProUGUI[] buffNameTexts;
    [SerializeField] private TextMeshProUGUI[] buffPriceTexts;

    [Space(50)]
    [Header("-------------------- REROLL --------------------")]
    [SerializeField] private Button rerollButton;
    private int rerollCost = 100;
    [SerializeField] private TextMeshProUGUI rerollCostText;
    private int remainingRerolls = 4;
    [SerializeField] private TextMeshProUGUI rerollCountText;

    [Space(50)]
    [Header("-------------------- BUY --------------------")]
    private int remainingBuyTurns = 2;
    [SerializeField] private TextMeshProUGUI buyTurnsText;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); 
    }

    void Start()
    {
        OnEnablePanel(false);
        closeLevelShopPanelButton.onClick.AddListener(() => OnEnablePanel(false));

        rerollButton.onClick.AddListener(() => rerollItems());
        rerollCountText.text = $"Rerolls Left: {remainingRerolls}";
        buyTurnsText.text = $"Buy Turns Left: {remainingBuyTurns}";

        for (int i = 0; i < weaponPurchaseButtons.Length; i++)
        {
            int index = i;  // Local copy of i to avoid closure issue in lambda??
            weaponPurchaseButtons[i].onClick.AddListener(() => OnWeaponBuyClicked(index));
        }

        for (int i = 0; i < buffPurchaseButtons.Length; i++)
        {
            int index = i;
            buffPurchaseButtons[i].onClick.AddListener(() => OnBuffBuyClicked(index));
        }


        GetRandomWeapon();
        GetRandomBuff();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetRandomWeapon();
            GetRandomBuff();
        }
    }

    public void OnEnablePanel(bool isEnable)
    {
        levelShopPanel.SetActive(isEnable);
    }


    // WEAPON ----------------------------------------------------------------------------------------------------------------------------------------------------
    private void GetRandomWeapon()
    {
        if (weaponItemList == null || weaponItemList.Count < 2)
        {
            Debug.LogWarning("WEAPON LIST IS EMPTY!!!");
            return;
        }

        // Filtered list
        List<WeaponItem> availableWeapons = new List<WeaponItem>(weaponItemList);

        //Debug.Log("Available Weapons Before: " + string.Join(", ", availableWeapons.Select(w => w.weaponName)));

        availableWeapons.RemoveAll(weapon => selectedWeapons.Contains(weapon));

        if (availableWeapons.Count < 2)
        {
            Debug.LogWarning("Not enough available weapons! Available weapons: " + string.Join(", ", availableWeapons.Select(w => w.weaponName)));
            return;
        } 
        else
        {
            //Debug.Log("Available Weapons After: " + string.Join(", ", availableWeapons.Select(w => w.weaponName)));
        }

        selectedWeapons[0] = availableWeapons[UnityEngine.Random.Range(0, availableWeapons.Count)];
        availableWeapons.Remove(selectedWeapons[0]);
        selectedWeapons[1] = availableWeapons[UnityEngine.Random.Range(0, availableWeapons.Count)];

        //Debug.Log("Selected Weapons: " + string.Join(", ", selectedWeapons.Select(w => w.weaponName)));

        SpawnWeaponInSlot(selectedWeapons[0], 0);
        SpawnWeaponInSlot(selectedWeapons[1], 1);
    }

    private void SpawnWeaponInSlot(WeaponItem weaponItem, int slotIndex)
    {
        if (weaponDisplaySlotArray[slotIndex].childCount != 0)
        {
            Destroy(weaponDisplaySlotArray[slotIndex].GetChild(0).gameObject);
        }
        GameObject weaponModel = Instantiate(weaponItem.weapon3DModel, weaponDisplaySlotArray[slotIndex].position, Quaternion.identity);
        weaponModel.transform.SetParent(weaponDisplaySlotArray[slotIndex], true);

        ShowWeaponInSlot(weaponItem, slotIndex);
    }

    private void ShowWeaponInSlot(WeaponItem weaponItem, int slotIndex)
    {
        weaponIcons[slotIndex].sprite = weaponItem.weaponSprite;
        weaponIcons[slotIndex].SetNativeSize();
        weaponNameTexts[slotIndex].text = weaponItem.weaponName;
        weaponPriceTexts[slotIndex].text = weaponItem.weaponPrice.ToString();
    }

    private void OnWeaponBuyClicked(int index)
    {
        int price = selectedWeapons[index].weaponPrice;
        if (wallet.DeductBioCompound(price) == true)
        {
            if (remainingBuyTurns > 0)
            {
                BuyWeapon(selectedWeapons[index]);
                remainingBuyTurns -= 1;
                buyTurnsText.text = $"Buy Turns Left: {remainingBuyTurns}";
                weaponPurchaseButtons[index].gameObject.SetActive(false);
                if (rerollButton.gameObject.activeSelf == true)
                {
                    rerollButton.gameObject.SetActive(false);
                    rerollCountText.text = "Rerolls Left: 0";
                }
            }
            else
            {
                Debug.Log("NO");
            }
        }
    }


    private void BuyWeapon(WeaponItem weaponItem)
    {
        for (int i = 0; i < itemSpawnSlotArray.Length; i++)
        {
            // Check if the slot doesn't have any child
            if (itemSpawnSlotArray[i].childCount == 0)
            {
                // Spawn the weapon in the empty slot
                GameObject weaponInstance = Instantiate(weaponItem.weaponPrefab, itemSpawnSlotArray[i].position, Quaternion.identity);
                weaponInstance.transform.SetParent(itemSpawnSlotArray[i], true);

                Debug.Log("Weapon spawned in slot: " + i);
                return;
            }
        }
        Debug.Log("No empty slots available to spawn the weapon.");
    }













    // BUFF ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void GetRandomBuff()
    {
        if (buffItems == null || buffItems.Count < 2)
        {
            Debug.LogWarning("Buff List is empty!");
            return;
        }

        List<BuffItem> availableBuffs = new List<BuffItem>(buffItems);
        availableBuffs.RemoveAll(buff => selectedBuffs.Contains(buff));

        if (availableBuffs.Count < 2)
        {
            Debug.LogWarning("Not enough available buffs! Available buffs: " + string.Join(", ", availableBuffs.Select(b => b.buffName)));
            return;
        }


        selectedBuffs[0] = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];
        availableBuffs.Remove(selectedBuffs[0]);
        selectedBuffs[1] = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];

        SpawnBuffInSlot(selectedBuffs[0], 0);
        SpawnBuffInSlot(selectedBuffs[1], 1);
    }

    private void SpawnBuffInSlot(BuffItem buffItem, int slotIndex)
    {
        // Check if the slot already has a child and destroy it
        if (buffDisplaySlotArray[slotIndex].childCount != 0)
        {
            Destroy(buffDisplaySlotArray[slotIndex].GetChild(0).gameObject);
        }

        GameObject buffModel = Instantiate(buffItem.buff3DModel, buffDisplaySlotArray[slotIndex].position, Quaternion.identity);
        buffModel.transform.SetParent(buffDisplaySlotArray[slotIndex], true);

        // Update the UI to show the buff
        ShowBuffInSlot(buffItem, slotIndex);
    }

    private void ShowBuffInSlot(BuffItem buffItem, int slotIndex)
    {
        buffIcons[slotIndex].sprite = buffItem.buffSprite;
        buffNameTexts[slotIndex].text = buffItem.buffName;
        buffPriceTexts[slotIndex].text = buffItem.buffPrice.ToString();
    }

    private void OnBuffBuyClicked(int index)
    {

        int price = selectedBuffs[index].buffPrice;

        if (wallet.DeductBioCompound(price) == true)
        {
            if (remainingBuyTurns > 0)
            {
                BuyBuff(selectedBuffs[index]);
                remainingBuyTurns -= 1;
                buyTurnsText.text = $"Buy Turns Left: {remainingBuyTurns}";
                buffPurchaseButtons[index].gameObject.SetActive(false);
                if (rerollButton.gameObject.activeSelf == true)
                {
                    rerollButton.gameObject.SetActive(false);
                    rerollCountText.text = "Rerolls Left: 0";
                }
            }
            else
            {
                Debug.Log("NO");
            }
        }
    }

    private void BuyBuff(BuffItem buffItem)
    {
        for (int i = 0; i < itemSpawnSlotArray.Length; i++)
        {
            // Check if the slot doesn't have any child
            if (itemSpawnSlotArray[i].childCount == 0)
            {
                GameObject buffInstance = Instantiate(buffItem.buffPrefab, itemSpawnSlotArray[i].position, Quaternion.identity);
                buffInstance.transform.SetParent(itemSpawnSlotArray[i], true); 

                Debug.Log("Buff spawned in slot: " + i);
                return;
            }
        }
        Debug.Log("No empty slots available to spawn the buff.");
    }








    private void rerollItems()
    {
        if (wallet.DeductBioCompound(rerollCost) == true)
        {
            if (remainingRerolls > 1)
            {
                GetRandomWeapon();
                GetRandomBuff();
                remainingRerolls -= 1;
                rerollCountText.text = $"Rerolls Left: {remainingRerolls}";
            }
            else if (remainingRerolls == 1)
            {
                remainingRerolls -= 1;
                rerollCountText.text = $"Rerolls Left: {remainingRerolls}";
                rerollButton.gameObject.SetActive(false);
            }
        }
    }
}
