using Manager;
using Cinemachine.Examples;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


namespace Player
{
    public class LockOnCameraHandler : MonoBehaviour
    {
        [Header("Camera Related Settings")]
        public CinemachineTargetGroup targetGroup;

        [SerializeField] private CinemachineVirtualCamera virtualCamera;



        [Header("Lock On Settings")]
        public CharacterManager currentLockOnTarget;
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockTarget;
        public CharacterManager rightLockTarget;
        public Transform virtualCameraTransform;

        [SerializeField] private Transform PlayerTransform;
        [SerializeField] private float maximumLockOnDistance = 30f;
        [SerializeField] private float minimumLockOnDistance = 5f;
        [SerializeField] private float transitionSpeed = 2f;
        [SerializeField] private LayerMask environmentLayer;

        private List<CharacterManager> availableTargets = new List<CharacterManager>();

        private InputHandler inputHandler;
        private PlayerManager playerManager;
        private CinemachineFramingTransposer framingTransposer;



        private void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            playerManager = GetComponent<PlayerManager>();
        }


        private void Start()
        {
            environmentLayer = LayerMask.GetMask("Environment");
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Update()
        {
            // HandleTrackedObjectOffset();
            MonitorLockOnDistance();
        }

        public void ToggleLockOnCamera()
        {
            virtualCamera.Priority = 11;
        }

        public void UnToggleLockOnCamera()
        {
            virtualCamera.Priority = 9;
        }

        //Adjusting the y axis of locked on camera based on how close the target is
        private void HandleTrackedObjectOffset()
        {
            if (currentLockOnTarget != null && framingTransposer != null)
            {
                float distance = Vector3.Distance(currentLockOnTarget.transform.position, PlayerTransform.position);

                // Define target offset values based on distance
                Vector3 targetOffset = distance <= minimumLockOnDistance ? new Vector3(0, 3f, 0) : new Vector3(0, 2f, 0);

                // Smoothly transition to the target offset
                framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(framingTransposer.m_TrackedObjectOffset, targetOffset, Time.deltaTime * transitionSpeed);
            }
        }




        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(PlayerTransform.position, 26);

            availableTargets.Clear();
            leftLockTarget = null;
            rightLockTarget = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager characterManager = colliders[i].GetComponent<CharacterManager>();

                // Skip if the target is dead
                if (characterManager != null)
                {
                    CharacterStats characterStats = characterManager.GetComponent<CharacterStats>();
                    if (characterStats != null && !characterStats.isDead)
                    {
                        Vector3 lockTargetDirection = characterManager.transform.position - PlayerTransform.position;
                        float distanceFromTarget = Vector3.Distance(PlayerTransform.position, characterManager.transform.position);
                        float viewableAngle = Vector3.Angle(lockTargetDirection, virtualCameraTransform.forward);
                        RaycastHit hit;

                        if (characterManager.transform.root != PlayerTransform.transform.root && viewableAngle > -180 && viewableAngle < 180 && distanceFromTarget <= maximumLockOnDistance)
                        {
                            if (Physics.Linecast(playerManager.lockOnTransform.position, characterManager.lockOnTransform.position, out hit))
                            {
                                if (hit.transform.gameObject.layer != environmentLayer)
                                {
                                    availableTargets.Add(characterManager);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < availableTargets.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(PlayerTransform.position, availableTargets[i].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i];
                }

                if (inputHandler.lockOnFlag)
                {
                    Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[i].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (relativeEnemyPosition.x <= 0 && distanceFromLeftTarget > shortestDistanceOfLeftTarget && availableTargets[i] != currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargets[i];
                    }
                    else if (relativeEnemyPosition.x >= 0 && distanceFromRightTarget < shortestDistanceOfRightTarget && availableTargets[i] != currentLockOnTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTargets[i];
                    }
                }
            }

            if (nearestLockOnTarget != null)
            {
                if (currentLockOnTarget != null)
                {
                    currentLockOnTarget.lockOnIndicator.SetActive(false);
                }

                currentLockOnTarget = nearestLockOnTarget;
                currentLockOnTarget.lockOnIndicator.SetActive(true);
            }
        }





        private void MonitorLockOnDistance()
        {
            if (currentLockOnTarget != null)
            {
                float distanceToTarget = Vector3.Distance(currentLockOnTarget.transform.position, PlayerTransform.position);
                if (distanceToTarget > maximumLockOnDistance)
                {
                    inputHandler.lockOn_Input = false;
                    inputHandler.lockOnFlag = false;
                    ClearLockOnTarget();
                    UnToggleLockOnCamera();
                }
            }
        }

        public void ClearLockOnTarget()
        {
            if (currentLockOnTarget != null)
            {
                currentLockOnTarget.lockOnIndicator.SetActive(false);
            }

            availableTargets.Clear();
            targetGroup.m_Targets[0].target = null;
            currentLockOnTarget = null;
            nearestLockOnTarget = null;
        }

    }
}
