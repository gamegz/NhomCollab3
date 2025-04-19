using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowMouse : MonoBehaviour
{
    #region Serializable
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _camera;

    Transform target;
    Vector2 playerV2;
    Vector2 mouseV2;
    Vector2 direction;
    Vector3 _offset;

    [Header("Value")]
    [SerializeField] private float _armMultiplier;
    #endregion

    private void Start()
    {
        _offset = new Vector3(2.30f, 14.93f, -8.73f);
    }

    void Update()
    {
        this.transform.position = _player.position + _offset;
        CameraWork();
    }

    void CameraWork()
    {
        // 1. Get the player position in screen space and mouse position
        playerV2 = Camera.main.WorldToScreenPoint(_player.position);
        mouseV2 = Input.mousePosition;

        // 2. Calculate the difference between player position and mouse position
        direction = (Commons.instance.RemapToScreenCenter(mouseV2) - Commons.instance.RemapToScreenCenter(playerV2));

        _camera.transform.localPosition = new Vector3(direction.x * _armMultiplier, 0, direction.y * _armMultiplier);
    }
}
