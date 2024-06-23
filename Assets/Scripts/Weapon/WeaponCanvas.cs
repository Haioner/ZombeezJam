using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCanvas : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currentBulletsText;
    [SerializeField] private TextMeshProUGUI inventoryBulletsText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Image weaponFill;

    [Header("DOT")]
    [SerializeField] private DOTweenAnimation currentBulletsDOT;

    private float currentReloadCooldown;

    private void Start()
    {
        float fillSpeedMultiplier = 1.2f;
        currentReloadCooldown = weaponController.statistics.ReloadTime * fillSpeedMultiplier;
    }

    private void OnEnable()
    {
        weaponController.OnBulletChanged += UpdateTexts;
        weaponController.OnReload += ReloadCooldown;
    }

    private void OnDisable()
    {
        weaponController.OnBulletChanged -= UpdateTexts;
        weaponController.OnReload -= ReloadCooldown;
    }

    private void Update()
    {
        CalculateReloadFill();
    }

    private void UpdateTexts(object sender, System.EventArgs e)
    {
        weaponIcon.sprite = weaponController.weaponSO.WeaponSprite;
        weaponFill.sprite = weaponController.weaponSO.WeaponSprite;

        currentBulletsDOT.DORestart();
        currentBulletsText.text = $"{weaponController.currentBullets}";
        inventoryBulletsText.text = $"<sprite=0>{weaponController.inventoryBullets}";

        currentReloadCooldown = weaponController.statistics.ReloadTime * 1.2f;
        weaponFill.fillAmount = 1f;
    }

    private void ReloadCooldown(object sender, System.EventArgs e)
    {
        currentReloadCooldown = 0;
    }

    private void CalculateReloadFill()
    {
        float fillSpeedMultiplier = 1.2f;
        float adjustedReloadTime = weaponController.statistics.ReloadTime * fillSpeedMultiplier;
        if (currentReloadCooldown < adjustedReloadTime)
        {
            currentReloadCooldown += Time.deltaTime;
            weaponFill.fillAmount = currentReloadCooldown / adjustedReloadTime;
        }
    }
}
