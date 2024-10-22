using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace PlayerUI
{
    public class UIManager : MonoBehaviour
    {
        public EquimentWindowUI equimentWindowUI;
        [SerializeField] private PlayerInventory playerInventory;

        [SerializeField] private TextMeshProUGUI healChargesText;

        [SerializeField] private GameObject boss;

        [Header("UI Window")]
        public GameObject playerHUD;
        public GameObject selectWindow;
        public GameObject equipmentScreenWindow;
        public GameObject weaponInventoryWindow;

        public GameObject pauseMenuWindow;

        [Header("Equiment Window Slot Selected")]
        public bool slot01Selected;
        public bool slot02Selected;


        [Header("Weapon Inventory")]
        [SerializeField] private GameObject weaponInventorySlotPrefab;
        [SerializeField] private Transform weaponInventorySlotsParent;
        private WeaponInventorySlot[] weaponInventorySlots;

        [Header("Pause Menu")]
        // [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image loadingFillImage;



        private void Start()
        {
            weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();

            equimentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
        }

        private void Update()
        {
            if (boss == null)
            {
                LoadEndingCutscene();
            }

        }

        //logic for adding inventory slots to UI from player inventory
        public void UpdateUI()
        {
            #region Weapon Inventory Slots
            for (int i = 0; i < weaponInventorySlots.Length; i++)
            {
                if (i < playerInventory.weaponsInventory.Count)
                {
                    if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count)
                    {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent);
                        weaponInventorySlots = weaponInventorySlotsParent.gameObject.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
                }
                else
                {
                    weaponInventorySlots[i].ClearInventorySlot();
                }
            }
            #endregion
        }

        public void OpenSelectWindow()
        {
            selectWindow.SetActive(true);
        }

        public void CloseSelectWindow()
        {
            selectWindow.SetActive(false);
        }

        public void CloseAllInventoryWindows()
        {
            ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            equipmentScreenWindow.SetActive(false);
            pauseMenuWindow.SetActive(false);
        }


        public void ResetAllSelectedSlots()
        {
            slot01Selected = false;
            slot02Selected = false;
        }

        public void Restart()
        {
            StartCoroutine(LoadLevel("Gameplay"));
        }

        public void Quit()
        {
            Application.Quit();
        }


        public void Menu()
        {
            StartCoroutine(LoadLevel("Menu"));
        }

        public void UpdateHealCharges(int charges)
        {
            healChargesText.text = charges.ToString(); // Update the UI text with remaining charges
        }

        public void LoadEndingCutscene()
        {
            StartCoroutine(LoadLevel("Ending"));
        }


        IEnumerator LoadLevel(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            loadingScreen.SetActive(true);

            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                loadingFillImage.fillAmount = progress;

                yield return null;
            }
        }

    }
}
