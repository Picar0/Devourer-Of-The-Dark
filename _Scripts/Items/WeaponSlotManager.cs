using Items;
using Player;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    public class WeaponSlotManager : MonoBehaviour
    {

        public WeaponItem attackingWeapon;

        WeaponHolderSlot leftHandSlot;
        WeaponHolderSlot rightHandSlot;

        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;


        QuickSlotsUI quickSlotsUI;

        PlayerStats playerStats;

        private void Awake()
        {
            quickSlotsUI = GameObject.FindGameObjectWithTag("QuickSlotsUI").GetComponent<QuickSlotsUI>();
            playerStats = GetComponentInParent<PlayerStats>();

            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
            }
            else
            {
                quickSlotsUI.UpdateWeaponQuickSlotsUI(weaponItem);
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
            }
        }

        #region Handle Weapon's Damage Collider

        //ToDo:  Remove  the left hand weapon logic
        private void LoadLeftWeaponDamageCollider()
        {
            if (leftHandSlot.currentWeaponModel != null)
            {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
        }

        private void LoadRightWeaponDamageCollider()
        {
            if (rightHandSlot.currentWeaponModel != null)
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
        }

        public void OpenRightDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void OpenLeftDamageCollider()
        {
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void CloseRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void CloseLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }


        #endregion


        #region Handle Weapon's Stamina Depletion

        //for light and heavy attack each have different multiplier
        public void DrainStaminaLightAttack()
        {
            playerStats.StaminaDepletion(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack()
        {
            playerStats.StaminaDepletion(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion

    }

}
