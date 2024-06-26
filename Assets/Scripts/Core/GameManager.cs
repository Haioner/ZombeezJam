using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int CurrentRoom { get; private set; }
    public int Coins { get; private set; }
    public event EventHandler OnCoinChanged;

    [Header("Room UI")]
    [SerializeField] private TextMeshProUGUI roomText;
    [SerializeField] private DOTweenAnimation roomAnimation;
    [SerializeField] private DOTweenAnimation cleanAnimation;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddCurrentRoom();
            Debug.Log("Current room : " + CurrentRoom);
        }
#endif
    }

    public void AddCoin(int value)
    {
        Coins += value;
        OnCoinChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SubtractCoin(int value)
    {
        Coins -= value;
        OnCoinChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddCurrentRoom()
    {
        CurrentRoom++;
    }

    public void CleanText()
    {
        if (!cleanAnimation.gameObject.activeInHierarchy)
            cleanAnimation.gameObject.SetActive(true);
        cleanAnimation.gameObject.transform.parent.transform.localScale = Vector3.one;
        cleanAnimation.DORestart();
    }

    public void NextRoomText()
    {
        roomText.text = "Room " + CurrentRoom.ToString();
        roomAnimation.DORestart();
    }
}
