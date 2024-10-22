using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Interactables : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual void Interact(PlayerManager playerManager)
        {
            //Debug.Log("You interacted with the object");
        }

    }
}
