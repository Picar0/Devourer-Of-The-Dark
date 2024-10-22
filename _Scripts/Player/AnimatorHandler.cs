using Manager;
using UnityEngine;

namespace Player
{
    public class AnimatorHandler : AnimatorManager
    {
        PlayerManager playerManager;
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        private int vertical;
        private int horizontal;

        public void Initialize()
        {
            playerManager = GetComponent<PlayerManager>();
            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            float v = Mathf.Clamp(verticalMovement, -1f, 1f);
            float h = Mathf.Clamp(horizontalMovement, -1f, 1f);

            v = Mathf.Round(v);
            h = Mathf.Round(h);

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }


        public void CanRotate()
        {
            anim.SetBool("canRotate", true);
        }

        public void StopRotation()
        {
            anim.SetBool("canRotate", false);
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }

        public void DisableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", false);
        }


        private void OnAnimatorMove()
        {
            if (!playerManager.isInteracting)
                return;

            float delta = Time.deltaTime;
            if (delta == 0)
                return; // Avoid division by zero

            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rigidbody.velocity = velocity;
        }
    }
}
