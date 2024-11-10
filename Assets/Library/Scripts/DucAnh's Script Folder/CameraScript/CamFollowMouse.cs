using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowMouse : MonoBehaviour
{
    #region Non-Serializable
    [Header("References")]
    private Transform bodyTransform;
    private Vector3 velocity = Vector3.zero;
    private Vector3 offSet;

    #endregion

    #region Serializable
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform camTransform;



    [Header("Values")]
    [SerializeField] private float maxDistance = 5f;       // Maximum offset distance from the player
    [SerializeField] private float timeToFollow = 0.1f;    // Camera follow time for smoothing
    //[SerializeField] private float followSpeed = 10f;      // A seperate value to use [UNUSED]
    #endregion

    void Start()
    {

        if (player == null)
        {
            Debug.Log("Player has not been assigned via Script or Serialized");
        }

        if (camTransform == null)
        {
            Debug.Log("camTransform has not been assigned via Script or Serialized");
        }
        else
        {
            offSet = camTransform.position - player.transform.position;
        }


    }

    void LateUpdate() 
    {
        if (camTransform != null)
        {

            bodyTransform = player.transform.Find("Body").GetComponent<Transform>();
            
            camTransform.position = bodyTransform.position + offSet;
            
            camTransform.rotation = Quaternion.identity;

            CameraWork();
        }
    }


    void CameraWork()
    {
        // 1. Get the player position in screen space and mouse position
        Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(bodyTransform.position);
        Vector2 mouseScreenPos = Input.mousePosition;

        // 2. Calculate the difference between player position and mouse position
        Vector2 difference = mouseScreenPos - playerScreenPos;

        // 3. Clamp the difference to limit how far the camera can be offset by the mouse
        if (difference.magnitude > maxDistance)
        {
            difference = difference.normalized * maxDistance;
        }

        // 4. Calculate the target position for the camera in world space
        Vector3 targetWorldPos = camTransform.position + (new Vector3(difference.x, 0, difference.y) * 1/2f);


        // 5. Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetWorldPos, ref velocity, timeToFollow);
        //transform.position = targetWorldPos;
    }
}
