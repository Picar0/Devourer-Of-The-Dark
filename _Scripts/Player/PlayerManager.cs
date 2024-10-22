using Manager;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PlayerManager : CharacterManager
    {
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        Animator animator;
        InteractableUI interactableUI;
        AnimatorHandler animatorHandler;

        public GameObject interactableGameObject;
        public GameObject interactablePopUp;
        public GameObject itemPopUp;

        public bool isInteracting;

        [Header("Player Flags")]
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;
        public bool isInvulnerable;

        private void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponent<AnimatorHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            animator = GetComponent<Animator>();
            interactableUI = interactableGameObject.GetComponent<InteractableUI>();
        }

        private void Update()
        {
            float delta = Time.deltaTime;

            isInteracting = animator.GetBool("isInteracting");
            canDoCombo = animator.GetBool("canDoCombo");
            isInvulnerable = animator.GetBool("isInvulnerable");
            animator.SetBool("isInAir", isInAir);

            inputHandler.TickInput(delta);
            animatorHandler.canRotate = animator.GetBool("canRotate");
            //this needs to be called here else it wont work as it really doesn't involve  rigid body instead it is root motion
            playerLocomotion.HandleEvade(delta);
            PlayerStats.instance.RegenerateStamina();

            CheckForInteractableObject();

        }

        private void FixedUpdate()
        {
            //Rigid body stuff 
            float delta = Time.deltaTime;

            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleRotation(delta);

        }

        private void LateUpdate()
        {
            //We doing this so buttons can be set to false at the end of frame which leads to good button registration
            inputHandler.evadeFlag = false;
            inputHandler.lightAttack_input = false;
            inputHandler.heavyAttack_input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.interact_Input = false;
            inputHandler.inventory_Input = false;


            if (isInAir)
            {
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }


        public void CheckForInteractableObject()
        {
            RaycastHit hit;

            // Original SphereCast to detect interactable objects in front of the player
            Vector3 rayOrigin = transform.position;
            rayOrigin.y = rayOrigin.y + 2f;

            // Added second SphereCast to keep triggering when player walks over interactable object
            bool hasHit = Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f) ||
                          Physics.SphereCast(rayOrigin, 0.3f, Vector3.down, out hit, 2.5f);

            if (hasHit && hit.collider.tag == "Interactable")
            {
                Interactables interactableObject = hit.collider.GetComponent<Interactables>();

                if (interactableObject != null)
                {
                    string interactableText = interactableObject.interactableText;
                    // Setting the UI text to the interactable object text
                    // Setting the text pop-up to true
                    interactableUI.interactableText.text = interactableText;
                    interactablePopUp.SetActive(true);

                    if (inputHandler.interact_Input)
                    {
                        hit.collider.GetComponent<Interactables>().Interact(this);
                    }
                }
            }
            else
            {
                if (interactablePopUp.activeSelf)
                {
                    interactablePopUp.SetActive(false);
                }

                if (inputHandler.interact_Input && itemPopUp != null)
                {
                    itemPopUp.SetActive(false);
                }
            }

        }




    }
}
