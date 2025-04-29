using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    private StatsUpgrade _playerStatsRef;
    private GameObject _playerRef;
    private BoxCollider _boxCollider;
    private float _expAmount;
    private bool isRoomEnd = false;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pickUpVolume;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.enabled = false;
        StartFloatingAnimation();
    }
    void Start()
    {
        
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void StartFloatingAnimation()
    {
        transform.DOMoveY(transform.position.y + 0.3f, 1f)
            .SetLoops(-1, LoopType.Yoyo) 
            .SetEase(Ease.InOutSine); 

        transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart) 
            .SetEase(Ease.Linear); 
    }

    void Update()
    {
        if(isRoomEnd && _playerStatsRef != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerRef.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    public void SetExp(float expAmount, StatsUpgrade playerStatsRef, GameObject playerRef)
    {
        _expAmount = expAmount;
        _playerStatsRef = playerStatsRef;
        _playerRef = playerRef;
    }

    public void AllowToMoveTowardPlayer()
    {
        isRoomEnd = true;
        _boxCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            transform.DOKill();
            GameManager.Instance.PlaySound(Sound.pickUpSound, pickUpVolume);
            _playerStatsRef.AddExp(_expAmount);
            Destroy(this.gameObject);
        }
    }
}
