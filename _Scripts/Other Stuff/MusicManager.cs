using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource musicSource;
    public AudioClip defaultMusic;
    public AudioClip bossMusic;
    public float fadeDuration = 1.0f; // Duration of fade out/in

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        // Start playing the default music
        PlayMusic(defaultMusic);
    }

    public void PlayMusic(AudioClip newClip)
    {
        StartCoroutine(FadeMusic(newClip));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out the current music
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in the new music
        musicSource.volume = 0;
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        musicSource.volume = startVolume;
    }

    public void SwitchToBossMusic()
    {
        PlayMusic(bossMusic);
    }

    public void SwitchToDefaultMusic()
    {
        PlayMusic(defaultMusic);
    }
}
