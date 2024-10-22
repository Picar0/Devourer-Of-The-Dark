using Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{

    public class WeaponPickup : Interactables
    {
        public WeaponItem weapon;


        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            //Pick up the item and add it to the player inventory
            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            AnimatorHandler animatorHandler;
            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            animatorHandler = playerManager.GetComponent<AnimatorHandler>();

            playerLocomotion.rigidbody.velocity = Vector3.zero; //To stop player moving when picking up item 
           // animatorHandler.PlayTargetAnimation("PickUpItem", true);
            playerInventory.weaponsInventory.Add(weapon);
            //setting the name of weapon here from weapon scriptable object 
            playerManager.itemPopUp.GetComponentInChildren<TextMeshProUGUI>().text = weapon.itemName;
            //Displaying the item that is picked
            playerManager.itemPopUp.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
            //popup for item box that will show picture and name of item picked
            playerManager.itemPopUp.SetActive(true);   
            Destroy(gameObject);

        }
    }
}
