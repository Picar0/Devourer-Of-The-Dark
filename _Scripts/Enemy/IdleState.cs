using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Enemy
{
    public class IdleState : State
    {
        public PursueTargetState pursueTargetState;
        [SerializeField] private UIBossHealth uiBossHealth;
        [SerializeField] private LayerMask detectionLayer;
        [SerializeField] private GameObject blocker;
        [SerializeField] private GameObject enemies;

        //Look for a potential target
        //Switch to the pursue target state if target is found
        //if not return this state
        //Allows the enemy to detect the target on detection layer
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {

            #region Handle Enemy Target Detection
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

                if (characterStats != null)
                {
                    //Check for Team ID
                    Vector3 targetDirection = characterStats.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.currentTarget = characterStats;
                        if (gameObject.CompareTag("Boss"))
                        {
                            uiBossHealth.SetUIHealthBarToActive();
                            MusicManager.instance.SwitchToBossMusic();
                            blocker.SetActive(true);
                            enemies.SetActive(false);
                        }
                    }
                }
            }
            #endregion

            #region Handle Transistion to next State

            if (enemyManager.currentTarget != null)
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }

            #endregion
        }
    }
}
