using System.Data;
using UnityEngine;

namespace KI_Project
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        private GameObject _targetTransformTarget;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform _myTransform;
        private Vector3 _cameraTransformPosition;
        private LayerMask _ignoreLayers;
        private Vector3 _cameraFollowVelocity = Vector3.zero;

        public static CameraHandler Singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float _targetPosition;
        private float _defaultPosition;
        private float _lookAngle;
        private float _pivotAngle;
        public float minimumPivotAngle = -35f;
        public float maximumPivotAngle = 35f;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffSet = 0.2f;
        public float minimumCollisionOffset = -0.2f;

        private void Awake()
        {
            Singleton = this;
            _myTransform = transform;
            _targetTransformTarget = GameObject.FindGameObjectWithTag("Player");
            targetTransform = _targetTransformTarget.transform;
            _defaultPosition = cameraTransform.localPosition.z;
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); // Camera ignore specific layers
        }

        public void FollowTarget(float delta) // Follow target, camera rotation around target
        {
            Vector3 targetPosition =
                Vector3.SmoothDamp(_myTransform.position, targetTransform.position, ref _cameraFollowVelocity, delta / followSpeed);

            _myTransform.position = targetPosition; // Set position to target position
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) // Handle camera rotations
        {
            _lookAngle += (mouseXInput * lookSpeed) / delta; // Add look angle
            _pivotAngle += ((mouseYInput * -1) * pivotSpeed) / delta; // Add pivot angle
            _pivotAngle =
                Mathf.Clamp(_pivotAngle, minimumPivotAngle,
                    maximumPivotAngle); // Clamp pivot angle, can not go higher of lower than minimum and maximum

            Vector3 rotation = Vector3.zero;
            rotation.y = _lookAngle; // Set rotation to look angle
            Quaternion targetRotation = Quaternion.Euler(rotation); // Create target rotation
            _myTransform.rotation = targetRotation; // Set rotation to target rotation

            rotation = Vector3.zero;
            rotation.x = _pivotAngle; // Set rotation to pivot angle

            targetRotation = Quaternion.Euler(rotation); // Create target rotation
            cameraPivotTransform.localRotation = targetRotation; // Set rotation to target rotation
        }
    }
}