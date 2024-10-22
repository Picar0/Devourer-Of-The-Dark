using System;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    public AudioSource enemyAudioSource;
    public SoundClip[] enemySounds;


    // Play a sound effect by name, similar to how your global AudioManager works
    public void PlayEnemySFX(string soundName)
    {
        SoundClip s = Array.Find(enemySounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound not found: " + soundName);
            return;
        }
        enemyAudioSource.PlayOneShot(s.clip);
    }
}
