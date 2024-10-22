using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PlayerUI
{
    public class HandEquipmentSlotUI : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        private WeaponItem weapon;

        [SerializeField] private Image icon;


        public bool handSlot01;
        public bool handSlot02;



        public void AddItem(WeaponItem newWeapon)
        {
            weapon = newWeapon;
            icon.sprite = weapon.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void RemoveItem()
        {
            weapon = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void SelectThisSlot()
        {
            if (handSlot01)
            {
                uiManager.slot01Selected = true;
            }
            else if (handSlot02)
            {
                uiManager.slot02Selected = true;
            }
        }



    }
}