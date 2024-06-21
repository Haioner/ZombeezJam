using UnityEngine.UI;
using UnityEngine;
using TMPro;

[System.Serializable]
public class CanvasWeaponStatistics
{
    public Slider WeaponTier;
    public TextMeshProUGUI TotalAmmoText;
    public TextMeshProUGUI DamageText;
    public TextMeshProUGUI SpreadText;
    public TextMeshProUGUI RateText;
    public TextMeshProUGUI AmmoAmountText;
    public TextMeshProUGUI ReloadTimeText;
}

public class WeaponDrop : MonoBehaviour, IInteractable
{
    [Header("Weapon")]
    [SerializeField] private WeaponSO weaponSO;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool IsNearest { get; set; }

    [Header("Statistics")]
    public WeaponStatistics statistics;
    public CanvasWeaponStatistics canvasWeaponStatistics;
    public CanvasWeaponStatistics canvasWeaponDropStatistics;

    [Header("Bullets")]
    [SerializeField] private int currentBullets = 0;
    [SerializeField] private int inventoryBullets;

    [Header("Canvas")]
    [SerializeField] private GameObject statsCanvas;

    private AudioSource audioSource;
    private WeaponController weaponController;
    private bool hasComparedValues;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        UpdateSprite();
        RandBullets();
        RandStatistics();
    }

    private void Update()
    {
        Outline();
        StatsCanvas();
    }

    #region Statistics
    private void RandStatistics()
    {
        statistics.WeaponTier = GetRandomTier();

        statistics.Damage = GetRandomStats(true, weaponSO.BulletDamage);
        statistics.Spread = GetRandomStats(false, weaponSO.Spread);
        statistics.Rate = GetRandomStats(true, weaponSO.Rate);
        statistics.AmmoAmount = GetIntRandomStats(true, weaponSO.CartridgeAmount);
        statistics.ReloadTime = GetRandomStats(false, weaponSO.ReloadCooldown);
    }

    private int GetRandomTier()
    {
        float randomValue = Random.value;
        return randomValue < 0.8f ? 1 : randomValue < 0.95f ? 2 : 3;
    }

    private float GetRandomStats(bool moreIsGood, float initialValue)
    {
        float randStats = initialValue;

        if (moreIsGood)
            randStats = Random.Range(initialValue * 0.5f, initialValue * 1.2f * statistics.WeaponTier);
        else
        {
            float subtractValue = Random.Range(initialValue * 0.3f * -statistics.WeaponTier, initialValue * 0.3f * statistics.WeaponTier);
            randStats -= subtractValue;
        }

        return randStats;
    }

    private int GetIntRandomStats(bool moreIsGood, int initialValue)
    {
        int randStats = initialValue;

        if (moreIsGood)
            randStats = Random.Range(initialValue / 2, initialValue * statistics.WeaponTier);
        else
        {
            int subtractValue = Random.Range(initialValue / 2, initialValue * statistics.WeaponTier);
            randStats -= subtractValue;
        }

        return randStats;
    }

    private void RandBullets()
    {
        int randCurrent = Random.Range(0, weaponSO.CartridgeAmount);
        currentBullets = randCurrent;

        int randInventory = Random.Range(0, 30);
        inventoryBullets = randInventory;
    }

    private void UpdateStatisticsUI()
    {
        float thisDamageMultipliedByBullets = statistics.Damage * weaponSO.BulletsPerShoot;
        float weaponDamageMultipliedByBullets = weaponController.statistics.Damage * weaponController.weaponSO.BulletsPerShoot;
        int totalWeaponAmmo = weaponController.inventoryBullets + weaponController.currentBullets;

        //Equipped
        canvasWeaponStatistics.WeaponTier.value = weaponController.statistics.WeaponTier;
        canvasWeaponStatistics.TotalAmmoText.text = "<sprite=" + 4.ToString() + "> " + totalWeaponAmmo.ToString("F0");
        canvasWeaponStatistics.DamageText.text = "POWER: " + weaponDamageMultipliedByBullets.ToString("F0");
        canvasWeaponStatistics.SpreadText.text = "SPREAD: " + weaponController.statistics.Spread.ToString("F1");
        canvasWeaponStatistics.RateText.text = "RATE: " + weaponController.statistics.Rate.ToString("F1");
        canvasWeaponStatistics.AmmoAmountText.text = "CARTRIDGE: " + weaponController.statistics.AmmoAmount.ToString("F0");
        canvasWeaponStatistics.ReloadTimeText.text = "RELOAD: " + weaponController.statistics.ReloadTime.ToString("F1");

        //Drop
        canvasWeaponDropStatistics.WeaponTier.value = statistics.WeaponTier;
        ComparateValues(true, inventoryBullets + currentBullets, totalWeaponAmmo, canvasWeaponDropStatistics.TotalAmmoText, "<sprite=4> ", "F0");
        ComparateValues(true, thisDamageMultipliedByBullets, weaponDamageMultipliedByBullets, canvasWeaponDropStatistics.DamageText, "POWER: ", "F0");
        ComparateValues(false, statistics.Spread, weaponController.statistics.Spread, canvasWeaponDropStatistics.SpreadText, "SPREAD: ", "F1");
        ComparateValues(true, statistics.Rate, weaponController.statistics.Rate, canvasWeaponDropStatistics.RateText, "RATE: ", "F1");
        ComparateValues(true, statistics.AmmoAmount, weaponController.statistics.AmmoAmount, canvasWeaponDropStatistics.AmmoAmountText, "CARTRIDGE: ", "F0");
        ComparateValues(false, statistics.ReloadTime, weaponController.statistics.ReloadTime, canvasWeaponDropStatistics.ReloadTimeText, "RELOAD: ", "F1");

    }

    private void ComparateValues(bool moreIsGood, float firstValue, float compareValue, TextMeshProUGUI statisticsText, string contentName, string stringFormat)
    {
        //statisticsText.text = "<sprite=" + spriteIndex.ToString() + "> " + firstValue.ToString(stringFormat);
        statisticsText.text = contentName + firstValue.ToString(stringFormat);

        if (moreIsGood)
        {
            if (firstValue > compareValue)
                statisticsText.color = Color.green;
            else if (firstValue == compareValue)
                statisticsText.color = Color.white;
            else
                statisticsText.color = Color.red;
        }
        else
        {
            if (firstValue > compareValue)
                statisticsText.color = Color.red;
            else if (firstValue == compareValue)
                statisticsText.color = Color.white;
            else
                statisticsText.color = Color.green;
        }
    }

    private void StatsCanvas()
    {
        if (weaponController == null)
            weaponController = FindObjectOfType<WeaponController>();

        statsCanvas.SetActive(IsNearest);

        if (IsNearest && !hasComparedValues)
        {
            UpdateStatisticsUI();
            hasComparedValues = true;
        }
        else if (!IsNearest)
        {
            hasComparedValues = false;
        }
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = weaponSO.WeaponSprite;

        if (weaponSO.itemHandType == ItemHandType.Single)
            spriteRenderer.transform.localPosition = new Vector3(0.275f, 0.133f, 0);
        else
            spriteRenderer.transform.localPosition = new Vector3(0.561f, 0.033f, 0);
    }
    #endregion

    #region Interact
    private void Outline()
    {
        if (IsNearest)
            spriteRenderer.sprite = weaponSO.WeaponOutlineSprite;
        else
            spriteRenderer.sprite = weaponSO.WeaponSprite;
    }

    public void Interact()
    {
        audioSource.PlayOneShot(weaponSO.EndReloadClip);

        if (weaponController == null)
            weaponController = FindObjectOfType<WeaponController>();

        //Save drop info
        WeaponSO lastWeapon = weaponSO;
        int lastCurrentBullets = currentBullets;
        int lastInventoryBullets = inventoryBullets;
        WeaponStatistics lastStatistics = statistics;

        //Change this drop
        weaponSO = weaponController.weaponSO;
        currentBullets = weaponController.currentBullets;
        inventoryBullets = weaponController.inventoryBullets;
        statistics = weaponController.statistics;
        UpdateSprite();

        //Change player weapon
        weaponController.SwitchWeapon(lastWeapon, lastCurrentBullets, lastInventoryBullets, lastStatistics);
        UpdateStatisticsUI();
    }
    #endregion
}
