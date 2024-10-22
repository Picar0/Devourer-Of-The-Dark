using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using PlayerUI;

namespace Player
{
    public class PlayerStats : CharacterStats
    {
        public static PlayerStats instance;
        private PlayerManager playerManager;
        private AnimatorHandler animatorHandler;

        [SerializeField] private ParticleSystem healEffect;
        [SerializeField] private float staminaRegenAmount = 5f;

        [SerializeField] private GameObject deathScreen;
        private float staminaRegenTimer = 0f;

        public bool IsInvulnerable
        {
            get { return playerManager.isInvulnerable; }
        }



        private void Awake()
        {
            instance = this;
            playerManager = GetComponent<PlayerManager>();
            animatorHandler = GetComponent<AnimatorHandler>();
        }

        private void Start()
        {
            //Setting max health and stamina as per player stats
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            PlayerHealthBar.Instance.SetMaxHealth(maxHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            PlayerStaminaBar.Instance.SetMaxStamina(maxStamina);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void TakeDamage(int damage)
        {

            currentHealth = currentHealth - damage;

            PlayerHealthBar.Instance.SetCurrentHealth(currentHealth);
            animatorHandler.PlayTargetAnimation("Damage-1", true);
            AudioManager.instance.PlayPlayerSFX("PlayerHurt");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
                animatorHandler.PlayTargetAnimation("Death-1", true);
                deathScreen.SetActive(true);
                Animator animator = deathScreen.GetComponentInChildren<Animator>();
                animator.Play("Death-Screen_Fade");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }


        public void StaminaDepletion(int staminaDrain)
        {
            currentStamina = currentStamina - staminaDrain;
            PlayerStaminaBar.Instance.SetCurrentStamina(currentStamina);
        }


        public void RegenerateStamina()
        {
            if (playerManager.isInteracting)
            {
                staminaRegenTimer = 0f;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;


                if (currentStamina < maxStamina && staminaRegenTimer > 1f)
                {
                    currentStamina += staminaRegenAmount * Time.deltaTime;
                    PlayerStaminaBar.Instance.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }

        }


        public void Heal(int healAmount)
        {
            if (isDead || currentHealth <= 0) return; // Do not heal if dead

            currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth); // Heal but do not exceed max health
            PlayerHealthBar.Instance.SetCurrentHealth(currentHealth); // Update health bar

            // Play Heal Effect
            healEffect.Play();
        }



    }
}
