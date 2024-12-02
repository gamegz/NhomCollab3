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
        if (other.CompareTag("Player"))
        {
            Debug.Log("Basketball People");
            Debug.Log("Gain" + buffData.buffName);
            Debug.Log("Gain" + buffData.type);
            PlayerDatas.Instance.GetStats.OnTriggerDamageBuff(buffData.type, 9999);
        }

    }
}
