using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneWayDoorBreakable : MonoBehaviour, IDamageable
{

    [Header("Reference")]
    [SerializeField] private GameObject _breakableDoorRef;
    private GameObject _player;
    //[SerializeField] private GameObject _animatedDoorRef; // Animation played immediately when spawned (will discuss with Tung later about this)


    [Header("Value")]
    [SerializeField] private float health = 50f; // really niggas?
    private bool hitByChargedATK = false;



    [Header("Coroutines")]
    private Coroutine destroyDoorCoroutine = null;


    private void Awake()
    {
        if (_breakableDoorRef == null)
            Debug.Log("BreakableDoorRef isn't assigned");
        else _breakableDoorRef.SetActive(true);

        //if (_animatedDoorRef == null)
        //    Debug.Log("AnimatedDoorRef isn't assigned");
        //else _animatedDoorRef.SetActive(false);
    }

    private void Start()
    { 
        _player = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void Update()
    {
        //Debug.Log(TargetInFront(_player.transform.position));
    }

    public void TakeDamage(int damageAmount) // No need to use damageAmount for now
    {
        if(!TargetInFront(_player.transform.position)) { return; }

        _breakableDoorRef.SetActive(false);

        if (destroyDoorCoroutine != null)
        {
            StopCoroutine(destroyDoorCoroutine);
            destroyDoorCoroutine = null;
        }

        if (destroyDoorCoroutine == null)
            destroyDoorCoroutine = StartCoroutine(DestroyDoor());
    }

    private bool TargetInFront(Vector3 target)
    {
        Vector3 toTarget = (target - transform.position).normalized;

        float dotProduct = Vector3.Dot(toTarget, transform.forward);

        if (dotProduct > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator DestroyDoor()
    {
        GameObject tempDoorPlaceholder = new GameObject("Destroyed Door Animation Placeholder");
        tempDoorPlaceholder.transform.parent = this.transform;
        tempDoorPlaceholder.transform.position = this.transform.position;

        yield return new WaitForSeconds(2f); // Temporary time for future animation implementation
        
        if (tempDoorPlaceholder != null)
            Destroy(tempDoorPlaceholder);

        destroyDoorCoroutine = null;
    }

}
