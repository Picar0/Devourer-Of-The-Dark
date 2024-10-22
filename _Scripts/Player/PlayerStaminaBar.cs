using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI
{
    public class PlayerStaminaBar : MonoBehaviour
    {
        public static PlayerStaminaBar Instance { get; private set; }
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private Slider mainSlider; // The actual stamina bar
        [SerializeField] private Slider easeSlider; // The delayed (ease) stamina bar
        [SerializeField] private float easeDelay = 1f; // Delay before the ease bar starts to decrease
        [SerializeField] private float easeLerpSpeed = 0.5f; // Speed at which the ease bar catches up

        private Coroutine easeStaminaCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        public void SetMaxStamina(float maxStamina)
        {
            mainSlider.maxValue = maxStamina;
            mainSlider.value = maxStamina;
            easeSlider.maxValue = maxStamina;
            easeSlider.value = maxStamina;
        }

        public void SetCurrentStamina(float currentStamina)
        {
            mainSlider.value = currentStamina;

            if (inputHandler.inventoryFlag) return; // Don't run the coroutine if the inventory is open

            if (easeStaminaCoroutine != null)
            {
                StopCoroutine(easeStaminaCoroutine);
            }

            easeStaminaCoroutine = StartCoroutine(EaseStaminaBarWithDelay());
        }


        private IEnumerator EaseStaminaBarWithDelay()
        {
            // Wait for the ease delay before starting the easing effect
            yield return new WaitForSeconds(easeDelay);

            while (easeSlider.value > mainSlider.value)
            {
                easeSlider.value = Mathf.MoveTowards(easeSlider.value, mainSlider.value, easeLerpSpeed * Time.deltaTime);
                yield return null;
            }

            easeSlider.value = mainSlider.value; // Ensure the final value matches exactly
            easeStaminaCoroutine = null; // Clear the reference once the coroutine is done
        }


        public void OnInventoryOpen()
        {
            if (easeStaminaCoroutine != null)
            {
                StopCoroutine(easeStaminaCoroutine);
                easeStaminaCoroutine = null;
            }
        }

        public void OnInventoryClose()
        {
            if (easeSlider.value > mainSlider.value)
            {
                easeStaminaCoroutine = StartCoroutine(EaseStaminaBarWithDelay());
            }
        }

    }
}
