using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class LevelMerchantPro : MonoBehaviour, IInteractable
{
    public static LevelMerchantPro Instance { get; private set; }

    [SerializeField] private Transform[] itemSpawnSlotArray;

    private int remainingBuyTurns = 2;
    public int RemainingBuyTurns => remainingBuyTurns; // Get
    public void ModifyRemainingBuyTurns(int amount) // Set
    {
        remainingBuyTurns += amount;
    }


    private PlayerWallet wallet;

    [Space(50)]
    [Header("-------------------- WEAPON --------------------")]
    [SerializeField] private List<WeaponItemPro> weaponItemProList;
    private WeaponItemPro[] selectedWeapons = new WeaponItemPro[2];
    [SerializeField] private WeaponSlot[] weaponSlotArray;


    [Space(50)]
    [Header("-------------------- BUFF --------------------")]
    [SerializeField] private List<BuffItemPro> buffItemProList;
    private BuffItemPro[] selectedBuffs = new BuffItemPro[2];
    [SerializeField] private BuffSlot[] buffSlotArray;


    [Space(50)]
    [Header("-------------------- REROLL --------------------")]
    private int rerollCost = 100;
    [SerializeField] private TextMeshPro rerollCostText;
    public int remainingRerolls = 4;
    [SerializeField] private TextMeshPro rerollCountText;





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
        wallet = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWallet>();
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
        if (wallet == null)
        {
            Debug.Log("This script tried to find a gameObject with the tag 'Player' but it seems you moron forgot to add it!");
        }
    }



    // WEAPON------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void GetRandomWeapon()
    {
        if (weaponItemProList == null || weaponItemProList.Count < 2)
        {
            Debug.LogWarning("WEAPON LIST IS EMPTY!!!");
            return;
        }

        // Filtered list
        List<WeaponItemPro> availableWeapons = new List<WeaponItemPro>(weaponItemProList);

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

        weaponSlotArray[0].GetWeapon(selectedWeapons[0]);
        weaponSlotArray[1].GetWeapon(selectedWeapons[1]);
    }






    // BUFF------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void GetRandomBuff()
    {
        if (buffItemProList == null || buffItemProList.Count < 2)
        {
            Debug.LogWarning("BUFF LIST IS EMPTY!!!");
            return;
        }

        // Filtered list
        List<BuffItemPro> availableBuffs = new List<BuffItemPro>(buffItemProList);

        //Debug.Log("Available Buffs Before: " + string.Join(", ", availableBuffs.Select(w => w.buffName)));

        availableBuffs.RemoveAll(buff => selectedBuffs.Contains(buff));

        if (availableBuffs.Count < 2)
        {
            Debug.LogWarning("Not enough available buffs! Available buffs: " + string.Join(", ", availableBuffs.Select(w => w.buffName)));
            return;
        }
        else
        {
            //Debug.Log("Available Buffs After: " + string.Join(", ", availableBuffs.Select(w => w.buffName)));
        }

        selectedBuffs[0] = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];
        availableBuffs.Remove(selectedBuffs[0]);
        selectedBuffs[1] = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];

        //Debug.Log("Selected Buffs: " + string.Join(", ", selectedBuffs.Select(w => w.buffName)));

        buffSlotArray[0].GetBuff(selectedBuffs[0]);
        buffSlotArray[1].GetBuff(selectedBuffs[1]);
    }



    public void OnInteract()
    {
        if (remainingRerolls > 0)
        {
            if (wallet.DeductBioCompound(rerollCost) == true)
            {
                GetRandomWeapon();
                remainingRerolls--;
                rerollCountText.text = $"Rerolls left: {remainingRerolls}";
            }
        }
        else
        {

        }
    }
}
