using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    [SerializeField] private GameObject standPoint;
    [SerializeField] private GameObject ElevatorPoint2;
    private bool _isPlayerOnPlatform = false;
    private float timeToMove = 1f;
    private bool allowToWork = false;
    private Coroutine currentCourotine;
    private BoxCollider _elevatorCollider;
    void Start()
    {
        _elevatorCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlayerOnPlatform)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, ElevatorPoint2.transform.position, timeToMove * Time.deltaTime);
            if(currentCourotine == null)
            {
                StartCoroutine(FinishGoingUp());
            }
        }
    }

    IEnumerator FinishGoingUp()
    {
        yield return new WaitForSeconds(3f);
        allowToWork = false;
        currentCourotine = null;
    }

    IEnumerator WaitForElevatorToRun()
    {
        yield return new WaitForSeconds(2f);
        allowToWork = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(WaitForElevatorToRun());
        } 
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (allowToWork)
            {
                other.transform.parent.SetParent(transform.parent);
                _isPlayerOnPlatform = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent.SetParent(null);
            _isPlayerOnPlatform = false;
            DontDestroyOnLoad(other.transform.parent.gameObject);
            _elevatorCollider.enabled = false;

        }
    }
}
