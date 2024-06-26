using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public class PlayerCanvas : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float progressSpeed;

    [Header("Stamina")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaFill;
    [SerializeField] private Color[] staminaColors;

    [Header("Coin")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private DOTweenAnimation coinDOT;

    private PlayerManager playerManager;
    private PlayerMovement playerMovement;
    private HealthController healthController;

    private void OnEnable()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        healthController = GetComponentInParent<HealthController>();

        healthSlider.maxValue = playerManager.MaxHealth;
        healthSlider.value = playerManager.MaxHealth;


        healthController.OnHealthChanged += MaxHealthValue;
    }

    private void OnDisable()
    {
        healthController.OnHealthChanged -= MaxHealthValue;
    }

    private void Start()
    {
        staminaSlider.maxValue = playerManager.MaxStamina;
        staminaSlider.value = playerManager.MaxStamina;

        GameManager.instance.OnCoinChanged += CoinUI;
        CoinUI(null, EventArgs.Empty);
    }

    private void MaxHealthValue(object sender, EventArgs e)
    {
        healthSlider.maxValue = playerManager.MaxHealth;
    }

    private void Update()
    {
        HealthSlider();
        StaminaSlider();
    }

    private void CoinUI(object sender, EventArgs e)
    {
        coinText.text = "<sprite=0>" + GameManager.instance.Coins.ToString();
        coinDOT.DORestart();
    }

    private void HealthSlider()
    {
        if (healthController == null) return;

        healthText.text = healthController.GetCurrentHealth().ToString("F0") + "/" + healthController.GetMaxHealth().ToString("F0");
        healthSlider.value = Mathf.MoveTowards(healthSlider.value, healthController.GetCurrentHealth(), SpeedProgress());
    }

    private float SpeedProgress()
    {
        return progressSpeed * (playerManager.MaxHealth / 5) * Time.deltaTime;
    }

    private void StaminaSlider()
    {
        if (playerMovement == null) return;

        staminaSlider.value = playerMovement.GetCurrentStamina();
        staminaFill.color = staminaSlider.value <= 10 ? staminaColors[1] : staminaColors[0];
    }
}
