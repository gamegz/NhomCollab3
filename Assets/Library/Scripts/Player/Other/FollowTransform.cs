using UnityEngine;

namespace Library.Scripts.Player.Other
{
    public class FollowTransform : MonoBehaviour
    {
        public Transform toFollow;

        private void Update()
        {
            if (toFollow)
            {
                transform.position = toFollow.position;
            }
        }
    }
}