using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{


    public class LockOnIndicator : MonoBehaviour
    {

        private Transform playerCameraTransform;

        private void Start()
        {
            // Get the Cinemachine FreeLook camera transform
            playerCameraTransform = Camera.main.transform;
        }

        void Update()
        {
            FacePlayer();
        }

        private void FacePlayer()
        {
            if (playerCameraTransform != null)
            {
                // Make the Lock On Indicator face the camera
                transform.LookAt(transform.position + playerCameraTransform.rotation * Vector3.forward,
                                 playerCameraTransform.rotation * Vector3.up);
            }
        }
    }
}
