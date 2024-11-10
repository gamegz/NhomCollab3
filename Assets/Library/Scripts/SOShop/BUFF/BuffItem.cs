using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Buff Item", menuName = "ScriptableObjects/Item/Buff Item")]
public class BuffItem : ScriptableObject
{
    public GameObject buffPrefab;
    public GameObject buff3DModel;
    public string buffName;
    public int buffPrice;
    public Sprite buffSprite;
}
