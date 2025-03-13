using UnityEngine;

namespace DefaultNamespace
{
    public class TeleportPortal: MonoBehaviour
    {
        public Transform targetPortal; // Set this in the Inspector to link portals

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) // Ensure the player has the tag "Player"
            {
                GameController.Instance.Teleport(other);
                Debug.Log("Player teleported to " + targetPortal.name);
            }
        }
    }
}