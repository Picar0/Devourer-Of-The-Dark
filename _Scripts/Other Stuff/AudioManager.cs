using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public SoundClip[] musicSound, sfxSound;
    public SoundClip[] footstepSounds;
    public AudioSource musicSource, playerSFXSource;

    private float footstepTimer = 0f;
    public float footstepInterval = 0.5f; // Time interval between footstep sounds

    private void Awake()
    {
        instance = this;
    }

    public void PlayMusic(string name)
    {
        SoundClip s = Array.Find(musicSound, x => x.name == name);
        if (s == null)
        {
            print("No Sound");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlayPlayerSFX(string name)
    {
        SoundClip s = Array.Find(sfxSound, x => x.name == name);
        if (s == null)
        {
            print("No Sound");
        }
        else
        {
            playerSFXSource.PlayOneShot(s.clip);
        }
    }

    public void PlayFootStepsSFX()
    {
        if (footstepSounds.Length > 0 && footstepTimer <= 0f)
        {
            int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Length);
            playerSFXSource.PlayOneShot(footstepSounds[randomIndex].clip);

            footstepTimer = footstepInterval; // Reset the footstep timer
        }
        else if (footstepSounds.Length == 0)
        {
            print("No Footstep Sounds found");
        }
    }

    private void Update()
    {
        if (footstepTimer > 0)
        {
            footstepTimer -= Time.deltaTime; // Decrease the footstep timer
        }
    }
}
