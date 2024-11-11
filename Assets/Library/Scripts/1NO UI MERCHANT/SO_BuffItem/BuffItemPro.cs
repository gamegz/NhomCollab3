using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Buff Item Pro", menuName = "ScriptableObjects/Item/Buff Item Pro")]
public class BuffItemPro : ScriptableObject
{
    public GameObject buffPrefab;
    public GameObject buffModel;
    public string buffName;
    public int buffBioCurrencyCost;
    public int buffCreditCurrencyCost;

}
