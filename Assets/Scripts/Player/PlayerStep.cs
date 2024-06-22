using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStep : MonoBehaviour
{
    [SerializeField] private AudioClip[] stepsClip;

    public void StepSoundEVENT()
    {
        int randClip = Random.Range(0, stepsClip.Length);
        float randPitch = Random.Range(0.9f, 1.1f);
        SoundManager.PlayAudioClip(stepsClip[randClip], randPitch);
    }
}
