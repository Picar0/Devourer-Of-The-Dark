using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyBossManager : MonoBehaviour
    {
        public string bossName;

        private EnemyStats enemyStats;
        [SerializeField] private UIBossHealth uiBossHealth;

        private void Awake()
        {
            enemyStats = GetComponent<EnemyStats>();
        }

        private void Start()
        {
            uiBossHealth.SetBossName(bossName);
            uiBossHealth.SetBossMaxHealth(enemyStats.maxHealth);
        }
    }
}
