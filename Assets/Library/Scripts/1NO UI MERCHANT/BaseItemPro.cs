using UnityEngine;

[CreateAssetMenu(fileName = "SO_Base Item Pro", menuName = "ScriptableObjects/Item/Base Item Pro")]
public class BaseItemPro : ScriptableObject
{
    public GameObject itemPrefab;
    public GameObject itemModel;
    public string itemName;
    public int itemBioCost;
    public int itemCreditCost;
}
