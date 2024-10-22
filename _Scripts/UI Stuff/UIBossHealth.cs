using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Enemy
{
    public class UIBossHealth : MonoBehaviour
    {
        public TextMeshProUGUI bossName;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider easeSlider;

        [SerializeField] private float easeDelay = 1f; // Delay before the ease bar starts to decrease
        [SerializeField] private float easeLerpSpeed = 0.5f; // Speed at which the ease bar catches up

        private float easeTimer = 0f; // Timer for the ease delay

        public void Start()
        {
            SetHealthBarToInactive();
        }

        public void SetBossName(string name)
        {
            bossName.text = name;
        }

        public void SetUIHealthBarToActive()
        {
            healthSlider.gameObject.SetActive(true);
            easeSlider.gameObject.SetActive(true);
        }

        public void SetHealthBarToInactive()
        {
            healthSlider.gameObject.SetActive(false);
            easeSlider.gameObject.SetActive(false);
        }

        public void SetBossMaxHealth(int maxHealth)
        {
            healthSlider.maxValue = maxHealth;
            easeSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            easeSlider.value = maxHealth;
        }

        public void SetBossCurrentHealth(int currentHealth)
        {
            healthSlider.value = currentHealth;
            easeTimer = easeDelay; // Reset the ease timer
        }

        private void Update()
        {
            // Easing logic for the easeSlider
            if (easeTimer > 0)
            {
                easeTimer -= Time.deltaTime;
            }
            else
            {
                easeSlider.value = Mathf.Lerp(easeSlider.value, healthSlider.value, easeLerpSpeed * Time.deltaTime);
            }
        }
    }
}
