using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation optionsDOT;
    [SerializeField] private AudioClip menuMusic;

    private void Start()
    {
        ReturnTimeScale();
        MusicManager.instance.ChangeMusic(menuMusic, 0);
    }

    public void PlayButtonMusicChange()
    {
        MusicManager.instance.BackToNormalMusic();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            optionsDOT.DORestart();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnTimeScale()
    {
        Time.timeScale = 1f;
    }
}
