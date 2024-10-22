using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Enemy
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        private EnemyManager enemyManager;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            enemyManager = GetComponent<EnemyManager>();
        }

        //Recenter our model when animator plays animation with root motion
        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidbody.velocity = velocity;
        }
    }
}
