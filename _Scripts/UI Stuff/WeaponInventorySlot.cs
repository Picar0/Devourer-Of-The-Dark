using Items;
using Player;
using Weapon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PlayerUI
{
    public class WeaponInventorySlot : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private WeaponSlotManager weaponSlotManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private WeaponEffect weaponEffect;
        [SerializeField] private Image icon;
        private WeaponItem item;


        public void AddItem(WeaponItem newItem)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(!false);
        }


        public void EquipThisItem()
        {
            //This Function is doing following things
            //1:Add current item to inventory 
            //2:Equip this new item
            //3:Remove this item from inventory

            //Added the item to the inventory and replaced the item in the slot
            if (uiManager.slot01Selected)
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponInRightHandSlots[0]);
                playerInventory.weaponInRightHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.slot02Selected)
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponInRightHandSlots[1]);
                playerInventory.weaponInRightHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else
            {
                return;
            }


            playerInventory.rightWeapon = playerInventory.weaponInRightHandSlots[playerInventory.currentRightWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            //Since we only using right hand slot 
            // weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon , true);

            // Call SwitchWeaponEffect to update the effect when a new weapon is equipped
            weaponEffect.SwitchWeaponEffect();

            uiManager.equimentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }



    }
}
