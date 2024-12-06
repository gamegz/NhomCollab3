using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LevelMerchantPro : MonoBehaviour, IInteractable
{
    public static LevelMerchantPro Instance { get; private set; }

    [SerializeField] private Transform[] itemSpawnSlotArray;

    public int remainingBuyTurns { get; private set; } = 2;
    public void ModifyRemainingBuyTurns(int amount)
    {
        remainingBuyTurns += amount;
    }

    [Space(25)]
    [Header("-------------------- WEAPON --------------------")]
    [SerializeField] private List<WeaponItemPro> weaponItemProList;
    private WeaponItemPro[] selectedWeapons = new WeaponItemPro[2];
    [SerializeField] private WeaponSlot[] weaponSlotArray;


    [Space(25)]
    [Header("-------------------- BUFF --------------------")]
    [SerializeField] private List<BuffItemPro> buffItemProList;
    private BuffItemPro[] selectedBuffs = new BuffItemPro[2];
    [SerializeField] private BuffSlot[] buffSlotArray;

    private bool isRandomBuffUnlocked = false;

    [Space(25)]
    [Header("-------------------- REROLL --------------------")]
    private int rerollCost = 100;
    [SerializeField] private TextMeshPro rerollCostText;
    [SerializeField] private TextMeshPro rerollCountText;

    public int remainingRerolls { get; private set; } = 4;
    public void ModifyRemainingRerolls(int amount)
    {
        remainingBuyTurns = amount;
    }



    public Transform[] ItemSpawnSlotArray
    {
        get { return itemSpawnSlotArray; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GetRandomWeapon();
        GetRandomBuff();
        UpdateRerollInfo();
    }

    public void UpdateRerollInfo()
    {
        rerollCostText.text = $"Cost: {rerollCost}";
        rerollCountText.text = $"Rerolls left: {remainingRerolls}";
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GetRandomWeapon();
            GetRandomBuff();
        }
    }


    // WEAPON------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void GetRandomWeapon()
    {
        if (weaponItemProList == null || weaponItemProList.Count < 2)
        {
            Debug.LogWarning("WEAPON LIST IS EMPTY OR TOO FEW!!!");
            return;
        }

        // Filtered list
        List<WeaponItemPro> availableWeapons = new List<WeaponItemPro>(weaponItemProList);
        availableWeapons.RemoveAll(weapon => selectedWeapons.Contains(weapon));

        selectedWeapons[0] = availableWeapons[UnityEngine.Random.Range(0, availableWeapons.Count)];
        availableWeapons.Remove(selectedWeapons[0]);
        selectedWeapons[1] = availableWeapons[UnityEngine.Random.Range(0, availableWeapons.Count)];

        weaponSlotArray[0].GetWeapon(selectedWeapons[0]);
        weaponSlotArray[1].GetWeapon(selectedWeapons[1]);
    }


    // BUFF------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void GetRandomBuff()
    {
        if (isRandomBuffUnlocked == true)
        {
            if (buffItemProList == null || buffItemProList.Count < 2)
            {
                Debug.LogWarning("BUFF LIST IS EMPTYOR TOO FEW!!!");
                return;
            }

            // Filtered list
            List<BuffItemPro> availableBuffs = new List<BuffItemPro>(buffItemProList);
            availableBuffs.RemoveAll(buff => selectedBuffs.Contains(buff));

            selectedBuffs[0] = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];
            availableBuffs.Remove(selectedBuffs[0]);
            selectedBuffs[1] = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];

            buffSlotArray[0].GetBuff(selectedBuffs[0]);
            buffSlotArray[1].GetBuff(selectedBuffs[1]);
        }
    }

    // Unlocks the random buff item when the player meets certain conditions (progress to a certain point).
    public void SetRandomBuffUnlocked(bool isUnlocked)
    {
        isRandomBuffUnlocked = isUnlocked;
    }

    public void OnInteract()
    {
        if (remainingRerolls > 0)
        {
            if (PlayerWallet.P_WalletInstance.DeductBioCompound(rerollCost) == true)
            {
                GetRandomWeapon();
                GetRandomBuff();
                remainingRerolls--;
                rerollCountText.text = $"Rerolls left: {remainingRerolls}";
            }
        }
    }
}
