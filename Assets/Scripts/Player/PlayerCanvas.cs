using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider healthSlider;

    [Header("Stamina")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaFill;
    [SerializeField] private Color[] staminaColors;

    private PlayerManager playerManager;
    private PlayerMovement playerMovement;
    private HealthController healthController;

    private void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        healthController = GetComponentInParent<HealthController>();

        healthSlider.maxValue = playerManager.MaxHealth;
        staminaSlider.maxValue = playerManager.MaxStamina;
    }

    private void Update()
    {
        HealthSlider();
        StaminaSlider();
    }

    private void HealthSlider()
    {
        if (healthController == null) return;

        healthSlider.value = healthController.GetCurrentHealth();
    }

    private void StaminaSlider()
    {
        if (playerMovement == null) return;

        staminaSlider.value = playerMovement.GetCurrentStamina();
        staminaFill.color = staminaSlider.value <= 10 ? staminaColors[1] : staminaColors[0];
    }
}
