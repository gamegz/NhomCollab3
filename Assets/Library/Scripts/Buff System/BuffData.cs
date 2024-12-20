using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Object", menuName = "Buff Data")]
public class BuffData : ScriptableObject
{
    public string buffName;
    public BuffType type;
    public float buffValue;
}
