using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Player;

namespace PlayerUI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        public static PlayerHealthBar Instance;
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private Slider mainSlider; // The actual health bar
        [SerializeField] private Slider easeSlider; // The delayed (ease) health bar
        [SerializeField] private float easeDelay = 1f; // Delay before the ease bar starts to decrease
        [SerializeField] private float easeLerpSpeed = 0.5f; // Speed at which the ease bar catches up

        private Coroutine easeHealthCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        public void SetMaxHealth(int maxHealth)
        {
            mainSlider.maxValue = maxHealth;
            mainSlider.value = maxHealth;
            easeSlider.maxValue = maxHealth;
            easeSlider.value = maxHealth;
        }

        public void SetCurrentHealth(int currentHealth)
        {
            mainSlider.value = currentHealth;

            if (inputHandler.inventoryFlag) return; // Don't run the coroutine if the inventory is open

            if (easeHealthCoroutine != null)
            {
                StopCoroutine(easeHealthCoroutine);
            }

            easeHealthCoroutine = StartCoroutine(EaseHealthBarWithDelay());
        }

        private IEnumerator EaseHealthBarWithDelay()
        {
            yield return new WaitForSeconds(easeDelay);

            while (easeSlider.value > mainSlider.value)
            {
                easeSlider.value = Mathf.MoveTowards(easeSlider.value, mainSlider.value, easeLerpSpeed * Time.deltaTime);
                yield return null;
            }

            easeSlider.value = mainSlider.value;
            easeHealthCoroutine = null;
        }

        public void OnInventoryOpen()
        {
            if (easeHealthCoroutine != null)
            {
                StopCoroutine(easeHealthCoroutine);
                easeHealthCoroutine = null;
            }
        }

        public void OnInventoryClose()
        {
            if (easeSlider.value > mainSlider.value)
            {
                easeHealthCoroutine = StartCoroutine(EaseHealthBarWithDelay());
            }
        }
    }
}
