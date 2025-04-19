using UnityEngine;

namespace Library.Scripts.Player.Other
{
    public class IndicatorHelper : MonoBehaviour
    {
        private Camera _camera;
        public Transform referencePoint;
        public Transform headPoint;
        public float estimatedCircleRadius = 1;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            referencePoint.rotation = Quaternion.identity;
            headPoint.rotation = Quaternion.identity;

            if (_camera)
            {
                var mousePos = Input.mousePosition;
                
                mousePos.z = (_camera.transform.position - referencePoint.position).magnitude;
                var worldPos = _camera.ScreenToWorldPoint(mousePos);
                worldPos.y = referencePoint.position.y;
                
                var targetDir = (worldPos - referencePoint.position).normalized;
                referencePoint.rotation = Quaternion.LookRotation(targetDir);

                var posOnRotation = GetPointOnCircle(referencePoint.position, referencePoint.rotation,
                    estimatedCircleRadius);

                posOnRotation.y = headPoint.position.y;
                headPoint.position = posOnRotation;
                headPoint.rotation = Quaternion.LookRotation(targetDir);
            }
        }
        
        private Vector3 GetPointOnCircle(Vector3 worldPosition, Quaternion rotation, float baseRadius = 1f)
        {
            var angle = rotation.eulerAngles.y * Mathf.Deg2Rad;
            var radius = baseRadius * (1f + Mathf.Abs(Mathf.Sin(rotation.eulerAngles.x * Mathf.Deg2Rad)));
        
            var x = worldPosition.x + radius * Mathf.Sin(angle);
            var z = worldPosition.z + radius * Mathf.Cos(angle);
        
            return new Vector3(x, worldPosition.y, z);
        }
    }
}   