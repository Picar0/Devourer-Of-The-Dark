using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class PursueTargetState : State
    {
        [SerializeField] private CombatStanceState combatStanceState;
        [SerializeField] private EnemyAudioManager enemyAudioManager; // Reference to the enemy's audio manager
        [SerializeField] private float footstepInterval = 0.5f; // Interval between footstep sounds

        private float footstepTimer = 0f; // Timer to track footstep sound interval

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            // Calculate distance from target
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            // Stop movement if performing an action and out of attack range
            if (enemyManager.isPerformingAction && distanceFromTarget > enemyManager.maximumAttackRange)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                enemyManager.enemyRigidbody.velocity = Vector3.zero;
                return this;
            }

            // Move towards target if within chase range
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            if (distanceFromTarget > enemyManager.maximumAttackRange)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);

                // Calculate movement direction and velocity
                targetDirection.Normalize();
                targetDirection.y = 0;
                targetDirection *= enemyManager.speed;
                Vector3 projectedVelocity = Vector3.ProjectOnPlane(targetDirection, Vector3.up);
                enemyManager.enemyRigidbody.velocity = projectedVelocity;

                // Handle footstep sounds
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    enemyAudioManager.PlayEnemySFX("FootstepSound"); // Ensure "FootstepSound" is assigned in EnemyAudioManager
                    footstepTimer = 0f; // Reset timer
                }
            }
            else
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                enemyManager.enemyRigidbody.velocity = Vector3.zero;
            }

            // Handle rotation towards target
            HandleRotateTowardsTarget(enemyManager);

            // Switch to combat stance if within attack range
            if (distanceFromTarget <= enemyManager.maximumAttackRange)
            {
                return combatStanceState;
            }
            else
            {
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            // Rotate manually
            if (enemyManager.isPerformingAction)
            {
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = enemyManager.transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
            }
            // Rotate with pathfinding (NavMesh)
            else
            {
                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);

                float distanceFromTarget = Vector3.Distance(enemyManager.transform.position, enemyManager.currentTarget.transform.position);
                float rotationToApplyToDynamicEnemy = Quaternion.Angle(enemyManager.transform.rotation, Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized));

                if (distanceFromTarget > 5)
                    enemyManager.navMeshAgent.angularSpeed = 500f;
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) < 30)
                    enemyManager.navMeshAgent.angularSpeed = 50f;
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) > 30)
                    enemyManager.navMeshAgent.angularSpeed = 500f;

                Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                Quaternion rotationToApplyToStaticEnemy = Quaternion.LookRotation(targetDirection);

                if (enemyManager.navMeshAgent.desiredVelocity.magnitude > 0)
                {
                    enemyManager.navMeshAgent.updateRotation = false;
                    enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation,
                        Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized), enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
                }
                else
                {
                    enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation, rotationToApplyToStaticEnemy, enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
                }
            }
        }
    }
}
