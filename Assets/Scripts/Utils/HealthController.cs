using System;
using UnityEngine;

public class HealthController : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [Header("Damage")]
    [SerializeField] private GameObject damageParticle;
    [SerializeField] private AudioClip damageClip;
    public event EventHandler OnDamaged;
    public event EventHandler OnDeath;

    [Header("Heal")]
    [SerializeField] private GameObject healParticle;
    [SerializeField] private AudioClip healClip;

    [Header("FloatNumber")]
    [SerializeField] private FloatNumber floatNumberPrefab;

    public float GetCurrentHealth() { return currentHealth; }
    public void SetNewMaxHealth(float maxHealthValue) => maxHealth = maxHealthValue;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Heal(float healValue)
    {
        if (currentHealth <= 0) return;

        if (currentHealth < maxHealth)
        {
            InstantiateFloatNumber(healValue, Color.green);
            currentHealth += healValue;
        }

        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(float damageValue)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damageValue;
            InstantiateFloatNumber(damageValue, Color.red);

            Vector3 spawnPosition = transform.position;
            spawnPosition.y += 1f;
            Instantiate(damageParticle, spawnPosition, Quaternion.identity);

            SoundManager.PlayAudioClipVolume(damageClip, 1f);
            OnDamaged?.Invoke(this, EventArgs.Empty);
        }

        if(currentHealth <= 0)
        {
            CheckDeath();
            currentHealth = 0;
        }
    }

    private void CheckDeath()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    private void InstantiateFloatNumber(float numberValue, Color textColor)
    {
        Vector3 randomOffset = UnityEngine.Random.insideUnitCircle * 0.5f;
        Vector3 spawnPosition = transform.position + randomOffset;
        spawnPosition.y += 1f;
        FloatNumber currentFloatNumber = Instantiate(floatNumberPrefab, spawnPosition, Quaternion.identity);
        currentFloatNumber.InitFloatNumber(numberValue, textColor);
    }
}
