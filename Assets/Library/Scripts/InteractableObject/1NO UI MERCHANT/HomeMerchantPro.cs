using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HomeMerchantPro : MonoBehaviour
{
    public static HomeMerchantPro Instance { get; private set; }

    [SerializeField] public Transform[] itemSpawnSlotArray { get; private set; }
    [SerializeField] private Transform freeWeaponSlot;
    [SerializeField] private TextMeshPro freeWeaponName;
     
    public int remainingBuyTurns { get; private set; } = 2;
    public void ModifyRemainingBuyTurns(int amount)
    {
        remainingBuyTurns += amount;
    }

    [Space(25)]
    [Header("-------------------- WEAPON --------------------")]
    [SerializeField] private List<WeaponItemPro> weaponItemProList;
    private WeaponItemPro[] selectedWeapons = new WeaponItemPro[3];
    [SerializeField] private WeaponSlotHome[] weaponSlotArray;


    [Space(25)]
    [Header("-------------------- BUFF --------------------")]
    [SerializeField] private List<BuffItemPro> buffItemProList;
    private BuffItemPro[] selectedBuffs = new BuffItemPro[2];
    [SerializeField] private BuffSlotHome[] buffSlotArray;


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
        if (weaponItemProList == null || weaponItemProList.Count < 3)
        {
            Debug.LogWarning("WEAPON LIST IS EMPTY OR TOO FEW!!!");
            return;
        }

        // Filtered list
        List<WeaponItemPro> availableWeapons = new List<WeaponItemPro>(weaponItemProList);
        availableWeapons.RemoveAll(weapon => selectedWeapons.Contains(weapon));

        // Remove any existing weapon objects from freeWeaponSlot before spawning a new weapon.
        for (int i = freeWeaponSlot.childCount - 1; i >= 0; i--)
        {
            Transform child = freeWeaponSlot.GetChild(i);
            if (child.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Destroy(child.gameObject);
            }
        }

        selectedWeapons[0] = availableWeapons[UnityEngine.Random.Range(0, availableWeapons.Count)];
        availableWeapons.Remove(selectedWeapons[0]);
        selectedWeapons[1] = availableWeapons[UnityEngine.Random.Range(0, availableWeapons.Count)];
        availableWeapons.Remove(selectedWeapons[1]);

        weaponSlotArray[0].GetWeapon(selectedWeapons[0]);
        weaponSlotArray[1].GetWeapon(selectedWeapons[1]);

        selectedWeapons[2] = availableWeapons[UnityEngine.Random.Range(0, availableWeapons.Count)];
        GameObject randomWeapon = Instantiate(selectedWeapons[2].itemPrefab, freeWeaponSlot.position, Quaternion.identity);
        randomWeapon.transform.SetParent(freeWeaponSlot, true);
        freeWeaponName.text = $"Free Weapon: {selectedWeapons[2]}";
    }


    // BUFF------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void GetRandomBuff()
    {
        if (buffItemProList == null || buffItemProList.Count < 2)
        {
            Debug.LogWarning("BUFF LIST IS EMPTY OR TOO FEW!!!");
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
