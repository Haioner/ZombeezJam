using Cinemachine;
using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    public CinemachineTargetGroup TargetGroup;
    public float WalkSpeed = 5f;
    public float RunSpeed = 10f;
    public float MaxStamina = 100f;
    public float StaminaSpeed = 10f;

    [Header("Light")]
    [SerializeField] private Transform lightTransform;

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
        anim.Play("Death");
    }
}
