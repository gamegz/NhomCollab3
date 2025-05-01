using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneWayDoorBreakable : MonoBehaviour, IDamageable
{

    [Header("Reference")]
    [SerializeField] private BoxCollider collider;

    public List<AudioClip> BarricadeBeingHitSounds = new List<AudioClip>();
    public List<AudioClip> BarricadeBeingBreakSounds = new List<AudioClip>();
    private GameObject _player;
    private AudioSource _audioSource;
    //[SerializeField] private GameObject _animatedDoorRef; // Animation played immediately when spawned (will discuss with Tung later about this)


    [Header("Value")]
    [SerializeField] private float health = 50f; // really niggas?
    [SerializeField] private bool breakableOneWay = false;
    private bool hitByChargedATK = false;
    


    [Header("Coroutines")]
    private Coroutine destroyDoorCoroutine = null;

    private int randomIndex = 0;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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
        randomIndex = Random.Range(0, BarricadeBeingHitSounds.Count);
        _audioSource.PlayOneShot(BarricadeBeingHitSounds[randomIndex]);
        if (breakableOneWay)
        {
            if(!TargetInFront(_player.transform.position)) { return; }
            health -= damageAmount;
            if(health > 0){return;}
            //_breakableDoorRef.SetActive(false);
            
            if (destroyDoorCoroutine != null)
            {
                StopCoroutine(destroyDoorCoroutine);
                destroyDoorCoroutine = null;
            }
    
            if (destroyDoorCoroutine == null)
                destroyDoorCoroutine = StartCoroutine(DestroyDoor());
        }
        else
        {
            health -= damageAmount;
            if(health > 0){return;}
            //_breakableDoorRef.SetActive(false);

            if (destroyDoorCoroutine != null)
            {
                StopCoroutine(destroyDoorCoroutine);
                destroyDoorCoroutine = null;
            }

            if (destroyDoorCoroutine == null)
                destroyDoorCoroutine = StartCoroutine(DestroyDoor());
        }
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
        //GameObject tempDoorPlaceholder = new GameObject("Destroyed Door Animation Placeholder");
        //tempDoorPlaceholder.transform.parent = this.transform;
        //tempDoorPlaceholder.transform.position = this.transform.position;
        randomIndex = Random.Range(0, BarricadeBeingBreakSounds.Count);
        _audioSource.PlayOneShot(BarricadeBeingBreakSounds[randomIndex]);
        collider.enabled = false;
        yield return new WaitForSeconds(2f); // Temporary time for future animation implementation
        Destroy(this.gameObject);
        //if (tempDoorPlaceholder != null)
        //    Destroy(tempDoorPlaceholder);

        destroyDoorCoroutine = null;
    }

}
