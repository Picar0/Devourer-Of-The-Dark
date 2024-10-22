using PlayerUI;
using UnityEngine;
using Cinemachine;

namespace Player
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool evadeInput;
        public bool interact_Input;
        public bool lightAttack_input;
        public bool heavyAttack_input;
        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;
        public bool inventory_Input;
        public bool heal_Input;
        public bool lockOn_Input;
        public bool rightLockTarget_Input;
        public bool leftLockTarget_Input;

        public bool evadeFlag;
        public bool comboFlag;
        public bool inventoryFlag;
        public bool lockOnFlag;


        [SerializeField] private CinemachineInputProvider cinemachineInputProvider;
        private PlayerManager playerManager;
        private PlayerControls inputActions;
        private PlayerAttack playerAttack;
        private PlayerInventory playerInventory;
        private UIManager uiManager;
        private LockOnCameraHandler lockOnCameraHandler;
        private PlayerStats playerStats;



        private Vector2 movementInput;
        private Vector2 cameraInput;
        private int healCharges = 10; // Max charges for healing


        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            playerAttack = GetComponent<PlayerAttack>();
            playerInventory = GetComponent<PlayerInventory>();
            playerStats = GetComponent<PlayerStats>();
            lockOnCameraHandler = GetComponent<LockOnCameraHandler>();
            uiManager = GameObject.FindWithTag("PlayerUI").GetComponent<UIManager>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }


        private void OnEnable()
        {
            //Putting all the inputs in on enable to reduce garbage collection
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
                // Attack input
                inputActions.PlayerActions.LightAttack.performed += i => lightAttack_input = true;
                inputActions.PlayerActions.HeavyAttack.performed += i => heavyAttack_input = true;
                //interact input
                inputActions.PlayerActions.Interact.performed += i => interact_Input = true;
                //Quick Slots input
                inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
                //Inventory input
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
                //Heal input
                inputActions.PlayerActions.Heal.performed += i => heal_Input = true;
                //LockOn input
                inputActions.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                //Right lock target
                inputActions.PlayerActions.LockOnTargetRight.performed += i => rightLockTarget_Input = true;
                //Left lock target
                inputActions.PlayerActions.LockOnTargetLeft.performed += i => leftLockTarget_Input = true;
            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            HandleMoveInput(delta);
            HandleEvadeInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotInput();
            HandleInventoryInput();
            HandleHealInput();
            HandleLockOn();
        }

        private void HandleMoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleEvadeInput(float delta)
        {

            if (inventoryFlag) return; // Prevent Evading if inventory is open

            //Reason it is not in OnEnable is because we need to triggered it on every frame instead one frame like other
            evadeInput = inputActions.PlayerActions.Evade.triggered;

            if (evadeInput)
            {
                evadeFlag = true;
            }
        }


        private void HandleAttackInput(float delta)
        {
            if (inventoryFlag) return; // Prevent attacking if inventory is open

            if (lightAttack_input)
            {
                if (playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttack.HandleLightAttackWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {

                    if (playerManager.isInteracting || playerManager.canDoCombo) return;
                    playerAttack.HandleLightAttack(playerInventory.rightWeapon);
                }

            }

            if (heavyAttack_input)
            {
                if (playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttack.HandleHeavyAttackWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {

                    if (playerManager.isInteracting || playerManager.canDoCombo) return;
                    playerAttack.HandleHeavyAttack(playerInventory.rightWeapon);
                }

            }
        }


        private void HandleQuickSlotInput()
        {
            if (d_Pad_Right)
            {
                playerInventory.ChangeRightWeapon();
            }

        }


        private void HandleInventoryInput()
        {
            if (inventory_Input)
            {
                inventoryFlag = !inventoryFlag;

                if (inventoryFlag)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.playerHUD.SetActive(false);
                    PlayerStaminaBar.Instance.OnInventoryOpen();
                    PlayerHealthBar.Instance.OnInventoryOpen();
                    cinemachineInputProvider.enabled = false;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.playerHUD.SetActive(true);
                    PlayerStaminaBar.Instance.OnInventoryClose();
                    PlayerHealthBar.Instance.OnInventoryClose();
                    cinemachineInputProvider.enabled = true;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }


        private void HandleHealInput()
        {
            if (heal_Input && healCharges > 0 && playerManager.isInteracting == false)
            {
                heal_Input = false; // Reset input after heal
                playerStats.Heal(50); // Heal for 70 health
                healCharges--;
                uiManager.UpdateHealCharges(healCharges); // Update the charges on the UI
                AudioManager.instance.PlayPlayerSFX("Heal");
            }
        }


        private void HandleLockOn()
        {
            if (inventoryFlag) return; // Prevent attacking if inventory is open

            if (lockOn_Input && lockOnFlag == false)
            {
                lockOn_Input = false;
                lockOnCameraHandler.HandleLockOn();
                if (lockOnCameraHandler.nearestLockOnTarget != null)
                {
                    if (lockOnCameraHandler.currentLockOnTarget != null)
                    {
                        // Disable the indicator for the previous target
                        lockOnCameraHandler.currentLockOnTarget.lockOnIndicator.SetActive(false);
                    }

                    lockOnCameraHandler.currentLockOnTarget = lockOnCameraHandler.nearestLockOnTarget;
                    lockOnCameraHandler.currentLockOnTarget.lockOnIndicator.SetActive(true); // Enable the indicator for the new target
                    lockOnCameraHandler.targetGroup.m_Targets[0].target = lockOnCameraHandler.currentLockOnTarget.transform;
                    lockOnCameraHandler.ToggleLockOnCamera();
                    lockOnFlag = true;
                }
            }
            else if (lockOn_Input && lockOnFlag)
            {
                lockOn_Input = false;
                lockOnFlag = false;
                lockOnCameraHandler.ClearLockOnTarget();
                lockOnCameraHandler.UnToggleLockOnCamera();
            }

            if (lockOnFlag && leftLockTarget_Input)
            {
                leftLockTarget_Input = false;
                lockOnCameraHandler.HandleLockOn();
                if (lockOnCameraHandler.leftLockTarget != null)
                {
                    if (lockOnCameraHandler.currentLockOnTarget != null)
                    {
                        // Disable the indicator for the previous target
                        lockOnCameraHandler.currentLockOnTarget.lockOnIndicator.SetActive(false);
                    }

                    lockOnCameraHandler.currentLockOnTarget = lockOnCameraHandler.leftLockTarget;
                    lockOnCameraHandler.currentLockOnTarget.lockOnIndicator.SetActive(true); // Enable the indicator for the new left target
                    lockOnCameraHandler.targetGroup.m_Targets[0].target = lockOnCameraHandler.currentLockOnTarget.transform;
                    lockOnCameraHandler.ToggleLockOnCamera();
                }
            }

            if (lockOnFlag && rightLockTarget_Input)
            {
                rightLockTarget_Input = false;
                lockOnCameraHandler.HandleLockOn();
                if (lockOnCameraHandler.rightLockTarget != null)
                {
                    if (lockOnCameraHandler.currentLockOnTarget != null)
                    {
                        // Disable the indicator for the previous target
                        lockOnCameraHandler.currentLockOnTarget.lockOnIndicator.SetActive(false);
                    }

                    lockOnCameraHandler.currentLockOnTarget = lockOnCameraHandler.rightLockTarget;
                    lockOnCameraHandler.currentLockOnTarget.lockOnIndicator.SetActive(true); // Enable the indicator for the new right target
                    lockOnCameraHandler.targetGroup.m_Targets[0].target = lockOnCameraHandler.currentLockOnTarget.transform;
                    lockOnCameraHandler.ToggleLockOnCamera();
                }
            }
        }

    }
}
