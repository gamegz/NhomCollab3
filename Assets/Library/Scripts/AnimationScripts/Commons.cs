using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Commons : MonoBehaviour
{
    public static Commons instance { get; private set; }
    [SerializeField] GameObject CameraParent;
 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Vector3 CameraParentForward()
    {
        return CameraParent.transform.forward;
    }

    public Vector2 GetMouseDir(Vector3 from) //Look at player mouse position
    {
        Vector2 fromV2 = Camera.main.WorldToScreenPoint(from);
        Vector2 mouseV2 = Input.mousePosition;
        return (RemapToScreenCenter(mouseV2) - RemapToScreenCenter(fromV2)).normalized;
    }

    public Vector2 RemapToScreenCenter(Vector2 pixelPosition)
    {
        float xNormalized = Mathf.Clamp((pixelPosition.x - Screen.width / 2) / (Screen.width / 2), -1, 1);
        float yNormalized = Mathf.Clamp((pixelPosition.y - Screen.height / 2) / (Screen.height / 2), -1, 1);

        return new Vector2(xNormalized, yNormalized);
    }

}
