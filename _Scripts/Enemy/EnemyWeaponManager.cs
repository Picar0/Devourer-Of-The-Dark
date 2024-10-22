using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyWeaponManager : MonoBehaviour
    {
        private DamageCollider damageCollider;

        // Start is called before the first frame update
        void Start()
        {
            damageCollider = GetComponentInChildren<DamageCollider>();
        }

        public void EnableDamageCollider()
        {
            damageCollider.EnableDamageCollider();
        }

        public void DisableDamageCollider()
        {
            damageCollider.DisableDamageCollider();
        }
    }
}
