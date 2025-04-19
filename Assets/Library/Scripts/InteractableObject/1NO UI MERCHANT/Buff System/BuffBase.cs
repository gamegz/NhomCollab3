using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBase : MonoBehaviour
{
    [SerializeField] BuffData buffData;


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    if (buffData.type == BuffType.Damage)
        //    {
        //        PlayerDatas.Instance.GetStats.OnTriggerDamageBuff(buffData.type, (int)buffData.buffValue);
        //    }
        //    else
        //    {
        //        PlayerDatas.Instance.GetStats.OnTriggerBuff(buffData.type, buffData.buffValue);
        //    }

        //}

    }
}
