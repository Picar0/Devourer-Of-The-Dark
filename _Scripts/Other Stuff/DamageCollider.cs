using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class DamageCollider : MonoBehaviour
    {
        private Collider damageCollider;
        [SerializeField] private int currentWeaponDamage = 25;
        [SerializeField] private GameObject bloodVFX;
        [SerializeField] private float vfxLifetime = 1f; // Time before the VFX is destroyed

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                if (playerStats != null && !playerStats.IsInvulnerable && !playerStats.isDead)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                    PlayBloodVFX(collision.ClosestPoint(transform.position), transform.forward);
                }
            }

            if (collision.tag == "Enemy")
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                    PlayBloodVFX(collision.ClosestPoint(transform.position), transform.forward);
                }
            }

            if (collision.tag == "Boss")
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                    PlayBloodVFX(collision.ClosestPoint(transform.position), transform.forward);
                }
            }
        }


        private void PlayBloodVFX(Vector3 hitPoint, Vector3 attackDirection)
        {
            // Instantiate the blood VFX at the hit point
            GameObject bloodEffect = Instantiate(bloodVFX, hitPoint, Quaternion.identity);

            // Rotate the blood VFX to align with the attack direction only on the Y axis
            Vector3 flatAttackDirection = new Vector3(attackDirection.x, 0, attackDirection.z);
            Quaternion rotation = Quaternion.LookRotation(flatAttackDirection);
            bloodEffect.transform.rotation = rotation;

            // Destroy the blood VFX after a certain amount of time
            Destroy(bloodEffect, vfxLifetime);
        }
    }
}
