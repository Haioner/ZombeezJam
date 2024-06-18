using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    public float WalkSpeed = 5f;
    public float RunSpeed = 10f;
    public float MaxStamina = 100f;
    public float StaminaSpeed = 10f;

    [Header("Health")]
    public float MaxHealth = 100f;

    [Header("Weapon")]
    public WeaponController weaponController;

    [Header("Animation")]
    public Animator anim;

    private HealthController healthController;
    private PlayerMovement playerMovement;

    private void OnEnable()
    {
        playerMovement = GetComponent<PlayerMovement>();
        healthController = GetComponent<HealthController>();

        healthController.OnDeath += Death;
    }

    private void OnDisable()
    {
        healthController.OnDeath -= Death;
    }

    private void Update()
    {
        UpdateWeaponAnim();
    }

    private void UpdateWeaponAnim()
    {
        switch (weaponController.weaponSO.itemHandType)
        {
            case ItemHandType.Single: anim.SetFloat("WeaponCount", 0); break;
            case ItemHandType.Double: anim.SetFloat("WeaponCount", 1); break;
        }
    }

    private void Death(object sender, EventArgs e)
    {
        playerMovement.enabled = false;
        weaponController.gameObject.SetActive(false);
        anim.Play("Death");
    }
}
