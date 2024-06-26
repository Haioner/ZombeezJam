using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource m_AudioSource;

    private static bool canPlayHeadShotClip = true;
    private static float headShotCooldown = 8f;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public static void PlayAudioClip(AudioClip clip)
    {
        m_AudioSource.pitch = 1f;
        m_AudioSource.PlayOneShot(clip);
    }

    public static void PlayAudioClip(AudioClip clip, float pitch)
    {
        m_AudioSource.pitch = pitch;
        m_AudioSource.PlayOneShot(clip);
    }

    public static void PlayAudioClipVolume(AudioClip clip, float volume)
    {
        m_AudioSource.pitch = 1f;
        m_AudioSource.volume = volume;
        m_AudioSource.PlayOneShot(clip);
    }

    public static void PlayHeadShotClip(AudioClip clip, float pitch)
    {
        if (canPlayHeadShotClip)
        {
            m_AudioSource.pitch = pitch;
            m_AudioSource.PlayOneShot(clip);
            canPlayHeadShotClip = false;
            m_AudioSource.gameObject.GetComponent<SoundManager>().StartCoroutine(HeadShotCooldown());
        }
    }

    private static IEnumerator HeadShotCooldown()
    {
        yield return new WaitForSeconds(headShotCooldown);
        canPlayHeadShotClip = true;
    }
}
