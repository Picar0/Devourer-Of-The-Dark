using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    public class CombatStanceState : State
    {
        public AttackState attackState;
        public PursueTargetState pursueTargetState;

        //Check for attack range
        //potentially cirlce  player or walk around them
        //if in attack range, switch to attack state
        // if we are in a cool down after attacking , return this state and continue circling player
        // if the player runs out of range , return the pursue target state
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            HandleRotateTowardsTarget(enemyManager);

            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                enemyManager.enemyRigidbody.velocity = Vector3.zero;
            }

            if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maximumAttackRange)
            {
                return attackState;
            }
            else if (distanceFromTarget > enemyManager.maximumAttackRange)
            {
                return pursueTargetState;
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
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
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