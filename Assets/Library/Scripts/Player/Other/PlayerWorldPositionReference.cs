using UnityEngine;

namespace Library.Scripts.Player.Other
{
    public class PlayerWorldPositionReference : MonoBehaviour
    {
        public Transform parent;
        [Space]
        public Transform frontSideTransform;
        public Transform backSideTransform;

        public Vector3 toGetPosition
        {
            get
            {
                if (!backSideTransform || !frontSideTransform || !parent) return transform.position;
                var rotation = parent.rotation.eulerAngles.y;

                if (rotation is > -90 and < 90)
                {
                    
                    return backSideTransform.position;
                }

                return frontSideTransform.position;
            }
        }
    }
}
    