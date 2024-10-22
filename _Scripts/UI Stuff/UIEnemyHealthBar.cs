using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class UIEnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider mainSlider; // The actual health bar
        [SerializeField] private Slider easeSlider; // The delayed (ease) health bar
        [SerializeField] private float timeUntilBarIsHidden = 0f; // Time after which the health bar hides
        [SerializeField] private float easeDelay = 1f; // Delay before the ease bar starts to decrease
        [SerializeField] private float easeLerpSpeed = 0.5f; // Speed at which the ease bar catches up

        private Transform playerCameraTransform;
        private float easeTimer = 0f;

        private void Start()
        {
            // Get the Cinemachine FreeLook camera transform
            playerCameraTransform = Camera.main.transform;
        }

        public void SetHealth(int health)
        {
            mainSlider.value = health;
            easeTimer = easeDelay; // Reset the ease timer
            timeUntilBarIsHidden = 3f; // Reset the timer to hide the health bar
        }

        public void SetMaxHealth(int maxHealth)
        {
            mainSlider.maxValue = maxHealth;
            easeSlider.maxValue = maxHealth;
            mainSlider.value = maxHealth;
            easeSlider.value = maxHealth;
        }

        private void Update()
        {
            timeUntilBarIsHidden -= Time.deltaTime;

            // Rotate the health bar to face the player
            FacePlayer();

            if (mainSlider != null)
            {
                if (timeUntilBarIsHidden <= 0)
                {
                    timeUntilBarIsHidden = 0;
                    HideHealthBars();
                }
                else
                {
                    if (!mainSlider.gameObject.activeSelf)
                        ShowHealthBars();
                }

                if (mainSlider.value <= 0)
                {
                    Destroy(mainSlider.gameObject);
                    Destroy(easeSlider.gameObject);
                }
            }

            if (easeSlider != null)
            {
                // Easing logic for the easeSlider
                if (easeTimer > 0)
                {
                    easeTimer -= Time.deltaTime;
                }
                else
                {
                    easeSlider.value = Mathf.Lerp(easeSlider.value, mainSlider.value, easeLerpSpeed * Time.deltaTime);
                }
            }
        }

        private void FacePlayer()
        {
            if (playerCameraTransform != null)
            {
                // Make the health bar face the camera
                transform.LookAt(transform.position + playerCameraTransform.rotation * Vector3.forward,
                                 playerCameraTransform.rotation * Vector3.up);
            }
        }

        private void ShowHealthBars()
        {
            mainSlider.gameObject.SetActive(true);
            easeSlider.gameObject.SetActive(true);
        }

        private void HideHealthBars()
        {
            mainSlider.gameObject.SetActive(false);
            easeSlider.gameObject.SetActive(false);
        }
    }
}
