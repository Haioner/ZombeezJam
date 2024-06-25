using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkinSelector : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image skinImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rateText;

    [Header("Skins")]
    [SerializeField] private Sprite[] skins;

    [Header("Stats")]
    [SerializeField] private CharacterStats[] charactersStats;
    private int currentSkin;

    private void Start()
    {
        DataManager.instance.LoadData();
        currentSkin = DataManager.instance.gameData.CurrentSkin;
        UpdateSkin();
        UpdateStats();
    }

    private void UpdateSkin()
    {
        skinImage.sprite = skins[currentSkin];
    }

    private void UpdateStats()
    {
        healthText.text = "Health " + charactersStats[currentSkin].MaxHealth.ToString("F0");
        speedText.text = "Speed " + charactersStats[currentSkin].MaxSpeed.ToString("F1");
        rateText.text = "Shoot Rate " + charactersStats[currentSkin].ShootRate.ToString("F1");
    }

    private void SaveGameData()
    {
        DataManager.instance.gameData.characterStats = charactersStats[currentSkin];
        DataManager.instance.gameData.CurrentSkin = currentSkin;
        DataManager.instance.SaveData();
    }

    public void NextSkin()
    {
        if (currentSkin < skins.Length - 1)
            currentSkin++;

        UpdateSkin();
        UpdateStats();
        SaveGameData();
    }

    public void PreviousSkin()
    {
        if (currentSkin > 0)
            currentSkin--;

        UpdateSkin();
        UpdateStats();
        SaveGameData();
    }
}
