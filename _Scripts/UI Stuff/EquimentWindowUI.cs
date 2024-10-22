using Player;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerUI
{
    public class EquimentWindowUI : MonoBehaviour
    {
        [SerializeField] private bool slot01Selected;
        [SerializeField] private bool slot02Selected;

        [SerializeField] private HandEquipmentSlotUI[] handEquipmentSlotUIs;


        private void Start()
        {

        }

        // All this is doing is just adding hand slot to the player inventory so that hand slots UI and player inventory can be connected and it is called at the start of ui manger 
        public void LoadWeaponOnEquipmentScreen(PlayerInventory playerInventory)
        {
            for (int i = 0; i < handEquipmentSlotUIs.Length; i++)
            {
                if (handEquipmentSlotUIs[i].handSlot01)
                {
                    handEquipmentSlotUIs[i].AddItem(playerInventory.weaponInRightHandSlots[0]);
                }
                else if (handEquipmentSlotUIs[i].handSlot02)
                {
                    handEquipmentSlotUIs[i].AddItem(playerInventory.weaponInRightHandSlots[1]);
                }
            }
        }


        public void SelectSlot01()
        {
            slot01Selected = true;
        }

        public void SelectSlot02()
        {
            slot02Selected = true;
        }



    }
}
