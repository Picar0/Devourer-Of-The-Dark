using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyManager : CharacterManager
    {
        private EnemyLocomotionManager enemyLocomotionManager;
        private EnemyAnimatorManager enemyAnimatorManager;
        private EnemyStats enemyStats;

        public NavMeshAgent navMeshAgent;
        public State currentState;
        public CharacterStats currentTarget;
        public Rigidbody enemyRigidbody;


        public bool isPerformingAction;
        public float rotationSpeed = 15f;
        public float maximumAttackRange = 1.5f; //Stoping distance to attack
        public float speed = 5f; //only used for generic character without root motion animation

        [Header("AI Settings")]
        public float detectionRadius = 20f;
        //The higher, and the lower, respectively these angles are, the greater the detection FOV (like eyesight)
        public float maximumDetectionAngle = 50f;
        public float minimumDetectionAngle = -50f;
        public float currentRecoveryTime = 0f;

        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            enemyStats = GetComponent<EnemyStats>();
            enemyRigidbody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        }

        private void Start()
        {
            navMeshAgent.enabled = false;
            enemyRigidbody.isKinematic = false;
        }

        private void Update()
        {
            HandleRecoveryTimer();
        }

        private void FixedUpdate()
        {
            HandleStateMachine();
        }

        private void HandleStateMachine()
        {
            if (enemyStats.isDead)
            {
                currentState = null;
                return;
            }

            if (currentState != null)
            {
                State nextState = currentState.Tick(this, enemyStats, enemyAnimatorManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }


        private void SwitchToNextState(State state)
        {
            currentState = state;
        }


        private void HandleRecoveryTimer()
        {

            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
            }
        }

    }
}
