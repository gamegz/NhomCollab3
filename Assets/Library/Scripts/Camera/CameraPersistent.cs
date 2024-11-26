using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPersistent : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < Object.FindObjectsOfType<CameraPersistent>().Length; i++)
        {
            if (Object.FindObjectsOfType<CameraPersistent>()[i] != this)
            {
                if (Object.FindObjectsOfType<CameraPersistent>()[i].name == gameObject.name)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
