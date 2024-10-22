using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    [CreateAssetMenu(menuName = "AI/Enemy Actions/Attack Action")]
    public class EnemyAttackAction : EnemyActions
    {
        public int attackScore = 3;
        public float recoveryTime = 2f;

        public float maximumAttackAngle = 35f;
        public float minimumAttackAngle = -35f;

        public float minimumDistanceToAttack = 0f;
        public float maximumDistanceToAttack = 3f;

    }
}
