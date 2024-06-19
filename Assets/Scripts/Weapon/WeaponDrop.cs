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
            randStats = Random.Range(initialValue * 0.3f, initialValue * 1.3f * statistics.WeaponTier);
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

        int randInventory = Random.Range(0, 100);
        inventoryBullets = randInventory;
    }

    private void UpdateStatisticsUI()
    {
        float thisDamageMultipliedByBullets = statistics.Damage * weaponSO.BulletsPerShoot;
        float weaponDamageMultipliedByBullets = weaponController.statistics.Damage * weaponController.weaponSO.BulletsPerShoot;

        //Equipped
        canvasWeaponStatistics.WeaponTier.value = weaponController.statistics.WeaponTier;
        int totalWeaponAmmo = weaponController.inventoryBullets + weaponController.currentBullets;
        canvasWeaponStatistics.TotalAmmoText.text = "<sprite=" + 4.ToString() + "> " + totalWeaponAmmo.ToString("F0");
        canvasWeaponStatistics.DamageText.text = "<sprite=" + 5.ToString() + "> " + weaponDamageMultipliedByBullets.ToString("F0");
        canvasWeaponStatistics.SpreadText.text = "<sprite=" + 3.ToString() + "> " + weaponController.statistics.Spread.ToString("F1");
        canvasWeaponStatistics.RateText.text = "<sprite=" + 0.ToString() + "> " + weaponController.statistics.Rate.ToString("F1");
        canvasWeaponStatistics.AmmoAmountText.text = "<sprite=" + 1.ToString() + "> " + weaponController.statistics.AmmoAmount.ToString("F0");
        canvasWeaponStatistics.ReloadTimeText.text = "<sprite=" + 2.ToString() + "> " + weaponController.statistics.ReloadTime.ToString("F1");

        //Drop
        canvasWeaponDropStatistics.WeaponTier.value = statistics.WeaponTier;
        ComparateValues(true, inventoryBullets + currentBullets,
            weaponController.inventoryBullets + weaponController.currentBullets, canvasWeaponDropStatistics.TotalAmmoText, 4, "F0");
        ComparateValues(true, thisDamageMultipliedByBullets, weaponDamageMultipliedByBullets, canvasWeaponDropStatistics.DamageText, 5, "F0");
        ComparateValues(false, statistics.Spread, weaponController.statistics.Spread, canvasWeaponDropStatistics.SpreadText, 3, "F1");
        ComparateValues(true, statistics.Rate, weaponController.statistics.Rate, canvasWeaponDropStatistics.RateText, 0, "F1");
        ComparateValues(true, statistics.AmmoAmount, weaponController.statistics.AmmoAmount, canvasWeaponDropStatistics.AmmoAmountText, 1, "F0");
        ComparateValues(false, statistics.ReloadTime, weaponController.statistics.ReloadTime, canvasWeaponDropStatistics.ReloadTimeText, 2, "F1");

    }

    private void ComparateValues(bool moreIsGood, float dropValue, float weaponValue, TextMeshProUGUI statisticsText, int spriteIndex, string stringFormat)
    {
        statisticsText.text = "<sprite=" + spriteIndex.ToString() + "> " + dropValue.ToString(stringFormat);

        if (moreIsGood)
        {
            if (dropValue > weaponValue)
                statisticsText.color = Color.green;
            else if (dropValue == weaponValue)
                statisticsText.color = Color.white;
            else
                statisticsText.color = Color.red;
        }
        else
        {
            if (dropValue > weaponValue)
                statisticsText.color = Color.red;
            else if (dropValue == weaponValue)
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
