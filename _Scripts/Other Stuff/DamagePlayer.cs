using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        if(PlayerStats.instance != null)
        {
            PlayerStats.instance.TakeDamage(damage);
        }
    }
}
