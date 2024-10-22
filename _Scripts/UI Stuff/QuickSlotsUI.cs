using Items;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI
{


    public class QuickSlotsUI : MonoBehaviour
    {
        [SerializeField] private Image weaponIcon;
        //[SerializeField] private Image ItemIcon;
        //[SerializeField] private Image MagicIcon;
        //[SerializeField] private Image healingIcon;


        //ToDo: Remove the null logic too as we will always have default weapon
        public void UpdateWeaponQuickSlotsUI (WeaponItem weapon)
        {
            if (weapon.itemIcon != null)
            {
                weaponIcon.sprite = weapon.itemIcon;
                weaponIcon.enabled = true;
            }
            else
            {
                weaponIcon.sprite = null; 
                weaponIcon.enabled = false;
            }
          
        }
    }
}
