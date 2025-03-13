using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameOverPlane: MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player")) // Ensure the player has the tag "Player"
            {
                GameController.Instance.StartOver();
            }
        }
    }
}