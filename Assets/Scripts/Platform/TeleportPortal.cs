using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TeleportPortal: MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player")) // Ensure the player has the tag "Player"
            {
                GameController.Instance.Teleport();
            }
        }
    }
}