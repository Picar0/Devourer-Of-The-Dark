using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;


namespace Enemy
{
    public class EnemyLocomotionManager : MonoBehaviour
    {
        private EnemyManager enemyManager;
        private EnemyAnimatorManager enemyAnimatorManager;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterCollisionBlocker;


        private void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        }


        private void Start()
        {
            Physics.IgnoreCollision(characterCollider, characterCollisionBlocker, true);
        }
    }

}