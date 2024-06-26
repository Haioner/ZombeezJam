using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource audioSource;
    private AudioClip initialMusic;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        initialMusic = audioSource.clip;
    }

    public void BackToNormalMusic(float fadeDuration = 1f)
    {
        ChangeMusic(initialMusic, fadeDuration);
    }

    public void ChangeMusic(AudioClip newMusic, float fadeDuration = 1f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMusic(newMusic, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip newMusic, float fadeDuration)
    {
        // Fade out
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newMusic;
        audioSource.Play();

        // Fade in
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        audioSource.volume = startVolume;
    }
}
