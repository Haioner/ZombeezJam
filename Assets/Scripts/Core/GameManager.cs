using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int CurrentRoom { get; private set; }

    [Header("Room UI")]
    [SerializeField] private TextMeshProUGUI roomText;
    [SerializeField] private DOTweenAnimation roomAnimation;
    [SerializeField] private DOTweenAnimation cleanAnimation;

    private void Awake()
    {
        instance = this;
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
