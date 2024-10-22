using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private LockOnCameraHandler lockOnCameraHandler;
        private PlayerManager playerManager;
        private Transform cameraObject;
        private InputHandler inputHandler;
        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        //public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")]
        [SerializeField] float groundDetectionRayStartPoint = 0.5f;
        [SerializeField] float minDistanceNeededToBeginFall = 1f;
        [SerializeField] float groundDirectionRayDistance = 0.2f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;



        [Header("Movement Stats")]
        [SerializeField] private float movementSpeed = 5.0f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] float fallingSpeed = 45;

        [Header("Roll Costs")]
        [SerializeField] private int rollStaminaCost = 15;
        [SerializeField] private int backStepStaminaCost = 12;


        public CapsuleCollider characterCollider;
        public CapsuleCollider characterCollisionBlocker;

        private void Awake()
        {
            lockOnCameraHandler = GetComponent<LockOnCameraHandler>();
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponent<AnimatorHandler>();
        }

        private void Start()
        {
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);

            Physics.IgnoreCollision(characterCollider, characterCollisionBlocker, true);
        }



        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        public void HandleRotation(float delta)
        {
            if (animatorHandler.canRotate)
            {
                //lock on rotation logic
                if (inputHandler.lockOnFlag)
                {
                    // In if statement if lock on camera is active then we will handle rotation in a way that allow player to strafe and roll with lock on
                    if (inputHandler.evadeFlag)
                    {
                        Vector3 targetDirection = Vector3.zero;
                        targetDirection = lockOnCameraHandler.virtualCameraTransform.forward * inputHandler.vertical;
                        targetDirection += lockOnCameraHandler.virtualCameraTransform.right * inputHandler.horizontal;

                        targetDirection.Normalize();
                        targetDirection.y = 0;

                        if (targetDirection == Vector3.zero)
                        {
                            targetDirection = transform.forward;
                        }

                        Quaternion tr = Quaternion.LookRotation(targetDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * delta);

                        transform.rotation = targetRotation;
                    }
                    else
                    {
                        Vector3 rotationDirection = moveDirection;
                        rotationDirection = lockOnCameraHandler.currentLockOnTarget.transform.position - transform.position;
                        rotationDirection.y = 0;
                        rotationDirection.Normalize();
                        Quaternion tr = Quaternion.LookRotation(rotationDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                        transform.rotation = targetRotation;
                    }

                }
                //normal rotation
                else
                {
                    Vector3 targetDir = Vector3.zero;
                    float moveOverride = inputHandler.moveAmount;

                    // Project the camera's forward and right vectors onto the horizontal plane
                    Vector3 cameraForward = cameraObject.forward;
                    cameraForward.y = 0;
                    cameraForward.Normalize();

                    Vector3 cameraRight = cameraObject.right;
                    cameraRight.y = 0;
                    cameraRight.Normalize();

                    targetDir = cameraForward * inputHandler.vertical;
                    targetDir += cameraRight * inputHandler.horizontal;

                    targetDir.Normalize();
                    targetDir.y = 0;

                    if (targetDir == Vector3.zero)
                        targetDir = myTransform.forward;

                    float rs = rotationSpeed;

                    Quaternion tr = Quaternion.LookRotation(targetDir);
                    Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

                    myTransform.rotation = targetRotation;
                }
            }

        }

        public void HandleMovement(float delta)
        {
            // when evading
            if (inputHandler.evadeFlag)
                return;

            // when falling or landing 
            if (playerManager.isInteracting)
                return;

            // Project the camera's forward and right vectors onto the horizontal plane
            Vector3 cameraForward = cameraObject.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraObject.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            moveDirection = cameraForward * inputHandler.vertical;
            moveDirection += cameraRight * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            // Play footstep sound when the player is moving
            if (inputHandler.moveAmount > 0 && !playerManager.isInteracting)
            {
                AudioManager.instance.PlayFootStepsSFX();
            }

            // Lock-on animations such as strafe, forward left and right, and backward
            if (inputHandler.lockOnFlag)
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal);
            }
            // Normal animations such as idle, walk, run
            else
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);
            }
        }


        public void HandleEvade(float delta)
        {
            // Prevent evading during interactions
            if (animatorHandler.anim.GetBool("isInteracting"))
            {
                return;
            }

            // Check if there's enough stamina to evade
            if (PlayerStats.instance.currentStamina <= 0)
            {
                return;
            }

            if (inputHandler.evadeFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0)
                {
                    // Play evade animation and sound
                    animatorHandler.PlayTargetAnimation("Evade", true);
                    moveDirection.y = 0;
                    Quaternion evadeRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = evadeRotation;
                    PlayerStats.instance.StaminaDepletion(rollStaminaCost);

                    // Play evade sound effect
                    AudioManager.instance.PlayPlayerSFX("EvadeSound");
                }
                else
                {
                    // Play step back animation and sound
                    animatorHandler.PlayTargetAnimation("StepBack", true);
                    PlayerStats.instance.StaminaDepletion(backStepStaminaCost);

                    // Play evade sound effect
                    AudioManager.instance.PlayPlayerSFX("EvadeSound");
                }
            }
        }


        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }


            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                rigidbody.AddForce(moveDirection * fallingSpeed / 15f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minDistanceNeededToBeginFall, Color.red, 0.1f, false);
            if (Physics.Raycast(origin, -Vector3.up, out hit, minDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 targetPos = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = targetPos.y;

                if (playerManager.isInAir)
                {
                    if (inAirTimer > 0.5)
                    {
                        Debug.Log("In Air for" + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Land", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }

            }
            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if (playerManager.isInAir == false)
                {
                    if (playerManager.isInteracting == false)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            if (playerManager.isGrounded)
            {
                if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, Time.deltaTime);
                }
                else
                {
                    myTransform.position = targetPosition;
                }
            }

        }

        #endregion
    }

}
