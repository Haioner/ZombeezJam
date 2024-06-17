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

    private float currentReloadCooldown;

    private void Start()
    {
        float fillSpeedMultiplier = 1.2f;
        currentReloadCooldown = weaponController.weaponSO.ReloadCooldown * fillSpeedMultiplier;
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

        currentBulletsText.text = $"{weaponController.currentBullets}";
        inventoryBulletsText.text = $"<sprite=0>{weaponController.inventoryBullets}";
    }

    private void ReloadCooldown(object sender, System.EventArgs e)
    {
        currentReloadCooldown = 0;
    }

    private void CalculateReloadFill()
    {
        float fillSpeedMultiplier = 1.2f;
        float adjustedReloadTime = weaponController.weaponSO.ReloadCooldown * fillSpeedMultiplier;
        if (currentReloadCooldown < adjustedReloadTime)
        {
            currentReloadCooldown += Time.deltaTime;
            weaponFill.fillAmount = currentReloadCooldown / adjustedReloadTime;
        }
    }
}
