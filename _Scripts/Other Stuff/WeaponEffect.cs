using System.Collections;
using Player;
using UnityEngine;

namespace Weapon
{
    public class WeaponEffect : MonoBehaviour
    {
        public WeaponHolderSlot weaponHolderSlot;

        public GameObject weaponEffect;

        private void Start()
        {
            StartCoroutine(WaitForWeaponModel());
        }

        private IEnumerator WaitForWeaponModel()
        {
            // Wait until currentWeaponModel is assigned
            while (weaponHolderSlot == null || weaponHolderSlot.currentWeaponModel == null)
            {
                yield return null; // Wait until the next frame
            }

            Transform currentWeaponModel = weaponHolderSlot.currentWeaponModel.transform;
            weaponEffect = FindChildWithTag(currentWeaponModel, "SlashEffect");
        }

        private GameObject FindChildWithTag(Transform parent, string tag)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
            }
            return null;
        }

        public void PlayWeaponEffect()
        {
            weaponEffect.SetActive(true);
        }

        public void StopWeaponEffect()
        {
            weaponEffect.SetActive(false);
        }

        public void SwitchWeaponEffect()
        {
            StartCoroutine(WaitForWeaponModel());
        }
    }
}
