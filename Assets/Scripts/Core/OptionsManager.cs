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
    [SerializeField] private Slider shootSlider;
    [SerializeField] private TextMeshProUGUI shootText;
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
        LoadShoot();
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

    //Music
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
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 40);
        PlayerPrefs.SetFloat("MVolume", value);
        musicText.text = value.ToString("F2");
    }

    //Sound
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
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(value) * 40);
        PlayerPrefs.SetFloat("SVolume", value);
        soundText.text = value.ToString("F2");
    }

    //Shoot
    private void LoadShoot()
    {
        if (PlayerPrefs.HasKey("ShVolume"))
        {
            float keyValue = PlayerPrefs.GetFloat("ShVolume");
            shootSlider.value = keyValue;
            shootText.text = keyValue.ToString("F2");
        }
        else
        {

        }
    }

    public void UpdateShootVolume(float value)
    {
        audioMixer.SetFloat("ShootVolume", Mathf.Log10(value) * 40);
        PlayerPrefs.SetFloat("ShVolume", value);
        shootText.text = value.ToString("F2");
    }
}
