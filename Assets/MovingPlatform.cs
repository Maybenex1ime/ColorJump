using UnityEngine;

namespace DefaultNamespace
{
    public class MovingPlatform: MonoBehaviour
    {
        public Vector3 moveDirection = Vector3.right; // Direction to move
        public float speed = 3f; // Movement speed
        public bool loopMovement = true; // Enable looping movement
        public float moveDistance = 5f; // Distance before turning back

        private Vector3 startPosition;
        private bool movingForward = true;

        void Start()
        {
            startPosition = transform.position; // Store initial position
        }

        void Update()
        {
            if (loopMovement)
            {
                // Ping-pong movement
                float offset = Mathf.PingPong(Time.time * speed, moveDistance);
                transform.position = startPosition + moveDirection.normalized * offset;
            }
            else
            {
                // Constant movement in one direction
                transform.position += moveDirection.normalized * speed * Time.deltaTime;
            }
        }
    }
}