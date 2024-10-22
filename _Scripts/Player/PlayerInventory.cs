using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;

        WeaponEffect weaponEffect;


        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponInRightHandSlots = new WeaponItem[1];
        public WeaponItem[] weaponInLeftHandSlots = new WeaponItem[1];

        public int currentRightWeaponIndex = -1;
        public int currentLeftWeaponIndex = -1;

        public List<WeaponItem> weaponsInventory;

        private void Awake()
        {
            weaponSlotManager = GetComponent<WeaponSlotManager>();
            weaponEffect = GetComponent<WeaponEffect>();
        }

        //ToDo: Comment out the left hand weapon logic as we will only have one arm weapon only
        private void Start()
        {
            rightWeapon = weaponInRightHandSlots[0];
            leftWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            //rightWeapon = weaponInRightHandSlots[currentRightWeaponIndex];
            //leftWeapon = weaponInLeftHandSlots[currentLeftWeaponIndex];
            //weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            //weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        //TODO we need to fix this logic first there should be no empty slots and second there will always be 1 default weapon instead of empty slot
        public void ChangeRightWeapon()
        {
            currentRightWeaponIndex = (currentRightWeaponIndex + 1) % weaponInRightHandSlots.Length;

            if (weaponInRightHandSlots[currentRightWeaponIndex] != null)
            {
                rightWeapon = weaponInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponInRightHandSlots[currentRightWeaponIndex], false);
            }

            // Call SwitchWeapon to update the effect when the weapon is switched
            weaponEffect.SwitchWeaponEffect();
        }


    }

}
