using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    public CinemachineTargetGroup TargetGroup;
    public float WalkSpeed = 5f;
    public float MaxStamina = 100f;
    public float StaminaSpeed = 10f;

    [Header("Light")]
    [SerializeField] private Transform lightTransform;

    [Header("Health")]
    public float MaxHealth = 100f;
    public Volume hitVolume;
    private float currentHitTimer;

    [Header("Weapon")]
    public WeaponController weaponController;
    public float MeeleDamage = 10f;

    [Header("Animation")]
    public Animator anim;

    private HealthController healthController;
    private PlayerMovement playerMovement;

    private void OnEnable()
    {
        playerMovement = GetComponent<PlayerMovement>();
        healthController = GetComponent<HealthController>();
        InitiateStats();

        healthController.OnDeath += Death;
        healthController.OnDamaged += OnHit;
    }

    private void OnDisable()
    {
        healthController.OnDeath -= Death;
        healthController.OnDamaged += OnHit;
    }

    private void Update()
    {
        UpdateWeaponAnim();
        HitVolumeTimer();
    }

    private void InitiateStats()
    {
        if (DataManager.instance == null) return;

        MaxStamina = DataManager.instance.gameData.characterStats.Stamina;
        WalkSpeed = DataManager.instance.gameData.characterStats.MaxSpeed;
        MaxHealth = DataManager.instance.gameData.characterStats.MaxHealth;
        healthController.SetNewMaxHealth(MaxHealth);
    }

    private void OnHit(object sender, EventArgs e)
    {
        hitVolume.weight = 1;
        CinemachineShake.instance.ShakeCamera(2f, 0.1f);
        currentHitTimer = 0;
    }

    private void HitVolumeTimer()
    {
        if (currentHitTimer < 0.07f)
        {
            currentHitTimer += Time.deltaTime;
        }
        else
        {
            hitVolume.weight = 0;
        }

    }

    private void LightWeaponPos(Vector3 position)
    {
        lightTransform.localPosition = position;
    }

    public void RunFov(bool isFov)
    {
        if (isFov)
            TargetGroup.m_Targets[0].radius = 7;
        else
            TargetGroup.m_Targets[0].radius = 6;
    }

    private void UpdateWeaponAnim()
    {
        switch (weaponController.weaponSO.itemHandType)
        {
            case ItemHandType.Single:
                LightWeaponPos(new Vector3(0.33f, 0.2f, 0));
                anim.SetFloat("WeaponCount", 0);
                break;

            case ItemHandType.Double:
                LightWeaponPos(new Vector3(0.55f, -0.03f, 0));
                anim.SetFloat("WeaponCount", 1);
                break;
        }
    }

    private void Death(object sender, EventArgs e)
    {
        playerMovement.enabled = false;
        weaponController.gameObject.SetActive(false);
        lightTransform.gameObject.SetActive(false);
        anim.Play("Death");
    }
}
