using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_", menuName = "ScriptableObjects/Data/ActorLayerData")]
public class ActorLayerData : ScriptableObject
{
    public LayerMask walkLayer;
    public LayerMask ignoreLayer;
    public LayerMask hostileTargetLayer;
    public LayerMask selfLayer;

}
