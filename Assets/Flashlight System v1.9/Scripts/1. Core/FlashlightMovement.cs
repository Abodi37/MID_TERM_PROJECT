using UnityEngine;

namespace FlashlightSystem
{
    public class FlashlightMovement : MonoBehaviour
    {
        private Vector3 v3Offset; // Offset between flashlight and camera at start
        private Transform followTransform; // The transform the flashlight will follow (main camera)
        private Transform myTransform; // Cached reference to this object's transform

        [SerializeField] private float _speed = 3.0f; // Rotation smoothing speed

        // Public property to get/set smoothing speed
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        void Start()
        {
            myTransform = transform; // Cache this object's transform
            followTransform = Camera.main.transform; // Get the main camera's transform
            v3Offset = myTransform.position - followTransform.position; // Calculate initial position offset
        }

        void Update()
        {
            // Match position with camera + offset
            transform.position = followTransform.position + v3Offset;

            // Smoothly rotate to match camera's rotation
            transform.rotation = Quaternion.Slerp(
                myTransform.rotation,
                followTransform.rotation,
                speed * Time.deltaTime
            );
        }
    }
}

