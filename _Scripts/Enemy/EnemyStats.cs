using Player;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class EnemyStats : CharacterStats
{
    private Animator animator;
    [SerializeField] private LockOnCameraHandler lockOnCameraHandler;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float delayForDestroy = 2f;
    public UIEnemyHealthBar uiEnemyHealthBar;
    [SerializeField] private UIBossHealth uiBossHealth;

    [SerializeField] private EnemyAudioManager enemyAudioManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAudioManager = GetComponent<EnemyAudioManager>();
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (gameObject.CompareTag("Boss"))
        {
            //dont do anything
        }
        else
        {
            uiEnemyHealthBar.SetMaxHealth(maxHealth);
        }

    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (gameObject.CompareTag("Boss"))
        {
            uiBossHealth.SetBossCurrentHealth(currentHealth);
        }
        else
        {
            uiEnemyHealthBar.SetHealth(currentHealth);
        }

        animator.Play("Damage");
        enemyAudioManager.PlayEnemySFX("Hurt");


        if (currentHealth <= 0)
        {
            if (gameObject.CompareTag("Boss"))
            {
                uiBossHealth.SetHealthBarToInactive();
                enemyAudioManager.PlayEnemySFX("Death");
                MusicManager.instance.SwitchToDefaultMusic();
            }
            currentHealth = 0;
            isDead = true;
            animator.Play("Death");
            inputHandler.lockOn_Input = false;
            inputHandler.lockOnFlag = false;
            lockOnCameraHandler.UnToggleLockOnCamera();
            lockOnCameraHandler.ClearLockOnTarget();
            StartCoroutine(DestroyAfterDeathAnimation());
        }
    }

    private IEnumerator DestroyAfterDeathAnimation()
    {
        yield return new WaitForSeconds(delayForDestroy);

        // Destroy the enemy object
        Destroy(gameObject);
    }


}

