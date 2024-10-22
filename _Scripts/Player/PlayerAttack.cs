using Items;
using Player;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

public class PlayerAttack : MonoBehaviour
{
    private InputHandler inputHandler;
    private AnimatorHandler animatorHandler;
    private WeaponSlotManager weaponSlotManager;

    private string lastAttack;
    [SerializeField] private int lightComboIndex;
    [SerializeField] private int heavyComboIndex;

    private void Awake()
    {
        animatorHandler = GetComponent<AnimatorHandler>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void HandleLightAttackWeaponCombo(WeaponItem weapon)
    {
        if (PlayerStats.instance.currentStamina <= 0)
        {
            return;
        }

        if (inputHandler.comboFlag && lightComboIndex < weapon.lightAttackAnimations.Count)
        {
            animatorHandler.anim.SetBool("canDoCombo", false);

            // If lastAttack matches the current animation, increment the combo index
            if (lastAttack == weapon.lightAttackAnimations[lightComboIndex])
            {
                lightComboIndex++;
                // If the index exceeds the list count, reset to 0
                if (lightComboIndex >= weapon.lightAttackAnimations.Count)
                {
                    lightComboIndex = 0;
                }
            }

            // Play the next animation in the combo sequence
            animatorHandler.PlayTargetAnimation(weapon.lightAttackAnimations[lightComboIndex], true);
            // Update lastAttack to the current animation
            lastAttack = weapon.lightAttackAnimations[lightComboIndex];

            // Play sword swinging sound
            AudioManager.instance.PlayPlayerSFX("SwordSwinging");
        }
    }

    public void HandleHeavyAttackWeaponCombo(WeaponItem weapon)
    {
        if (PlayerStats.instance.currentStamina <= 0)
        {
            return;
        }

        if (inputHandler.comboFlag && heavyComboIndex < weapon.heavyAttackAnimations.Count)
        {
            animatorHandler.anim.SetBool("canDoCombo", false);

            // If lastAttack matches the current animation, increment the combo index
            if (lastAttack == weapon.heavyAttackAnimations[heavyComboIndex])
            {
                heavyComboIndex++;
                // If the index exceeds the list count, reset to 0
                if (heavyComboIndex >= weapon.heavyAttackAnimations.Count)
                {
                    heavyComboIndex = 0;
                }
            }

            // Play the next animation in the combo sequence
            animatorHandler.PlayTargetAnimation(weapon.heavyAttackAnimations[heavyComboIndex], true);
            // Update lastAttack to the current animation
            lastAttack = weapon.heavyAttackAnimations[heavyComboIndex];

            // Play sword swinging sound
            AudioManager.instance.PlayPlayerSFX("SwordSwinging");
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        if (PlayerStats.instance.currentStamina <= 0)
        {
            return;
        }

        weaponSlotManager.attackingWeapon = weapon;
        animatorHandler.PlayTargetAnimation(weapon.lightAttackAnimations[0], true);
        lastAttack = weapon.lightAttackAnimations[0];
        lightComboIndex = 0; // Reset combo index for new light attack

        // Play sword swinging sound
        AudioManager.instance.PlayPlayerSFX("SwordSwinging");
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (PlayerStats.instance.currentStamina <= 0)
        {
            return;
        }

        weaponSlotManager.attackingWeapon = weapon;
        animatorHandler.PlayTargetAnimation(weapon.heavyAttackAnimations[0], true);
        lastAttack = weapon.heavyAttackAnimations[0];
        heavyComboIndex = 0; // Reset combo index for new heavy attack

        // Play sword swinging sound
        AudioManager.instance.PlayPlayerSFX("SwordSwinging");
    }
}
