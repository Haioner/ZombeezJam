using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private KeyCode optionsInput;

    [Header("Animations")]
    [SerializeField] private DOTweenAnimation gamePausedDOT;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [Space]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicText;
    [Space]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private TextMeshProUGUI soundText;

    private CanvasGroup canvasGroup;
    private bool optionsIsOn;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        LoadMusic();
        LoadSound();
    }

    private void Update()
    {
        if (Input.GetKeyDown(optionsInput))
        {
            SwitchOptions();
        }
    }

    public bool GetOptionsState()
    {
        return optionsIsOn;
    }

    public void SwitchOptions()
    {
        optionsIsOn = !optionsIsOn;
        Time.timeScale = optionsIsOn ? 0 : 1;
        gamePausedDOT.DORestart();
        SwitchCanvasGroup();
    }

    private void SwitchCanvasGroup()
    {
        if (optionsIsOn)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void BackToMenu()
    {
        TransitionController.instance.TransitionToSceneName("Menu");
    }

    private void LoadMusic()
    {
        if (PlayerPrefs.HasKey("MVolume"))
        {
            float keyValue = PlayerPrefs.GetFloat("MVolume");
            musicSlider.value = keyValue;
            musicText.text = keyValue.ToString("F2");
        }
    }

    public void UpdateMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MVolume", value);
        musicText.text = value.ToString("F2");
    }

    private void LoadSound()
    {
        if (PlayerPrefs.HasKey("SVolume"))
        {
            float keyValue = PlayerPrefs.GetFloat("SVolume");
            soundSlider.value = keyValue;
            soundText.text = keyValue.ToString("F2"); 
        }
    }

    public void UpdateSoundVolume(float value)
    {
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SVolume", value);
        soundText.text = value.ToString("F2");
    }
}
