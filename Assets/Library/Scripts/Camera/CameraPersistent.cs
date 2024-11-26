using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPersistent : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
