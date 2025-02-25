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
        direction = (RemapToScreenCenter(mouseV2) - RemapToScreenCenter(playerV2));

        _camera.transform.localPosition = new Vector3(direction.x * _armMultiplier, 0, direction.y * _armMultiplier);
    }

    public Vector2 RemapToScreenCenter(Vector2 pixelPosition)
    {
        float xNormalized = Mathf.Clamp((pixelPosition.x - Screen.width / 2) / (Screen.width / 2), -1, 1);
        float yNormalized = Mathf.Clamp((pixelPosition.y - Screen.height / 2) / (Screen.height / 2), -1, 1);

        return new Vector2(xNormalized, yNormalized);
    }
}
