using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Quaternion = System.Numerics;
using Vector3 = UnityEngine.Vector3;

namespace KI_Project
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform _cameraObject;
        InputHandler _inputHandler;
        Vector3 _moveDirection;
        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;
        [Header("Stats")] [SerializeField] float movementSpeed = 5f;
        [SerializeField] float rotationSpeed = 10f;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            Debug.Assert(Camera.main != null, "Camera.main != null");
            _cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
        }

        public void Update()
        {
            float delta = Time.deltaTime;
            _inputHandler.TickInput(delta);

            _moveDirection = _cameraObject.forward * _inputHandler.vertical;
            _moveDirection += _cameraObject.right * _inputHandler.horizontal;
            _moveDirection.Normalize();
            //_moveDirection.y = 0;

            float speed = movementSpeed;
            _moveDirection *= speed;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
            rigidbody.velocity = projectedVelocity;
            
            animatorHandler.UpdateAnimatorValues(_inputHandler.moveAmount, 0);
            
            //enables Rotation if true
            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        #region "Movement"

        Vector3 _normalVector;
        Vector3 _targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = _inputHandler.moveAmount;

            targetDir = _cameraObject.forward * _inputHandler.vertical;
            targetDir += _cameraObject.right * _inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            var rs = rotationSpeed;

            UnityEngine.Quaternion tr = UnityEngine.Quaternion.LookRotation(targetDir);
            UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        #endregion
    }
}