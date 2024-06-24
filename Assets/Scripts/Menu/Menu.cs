using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation optionsDOT;

    private void Start()
    {
        ReturnTimeScale();
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
