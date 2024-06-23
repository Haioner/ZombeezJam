using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [Header("Damage")]
    [SerializeField] private Blood blood;
    [SerializeField] private GameObject damageParticle;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip headShotClip;
    public event EventHandler OnDamaged;
    public event EventHandler OnDeath;

    [Header("Heal")]
    [SerializeField] private GameObject healParticle;
    [SerializeField] private AudioClip healClip;

    [Header("FloatNumber")]
    [SerializeField] private FloatNumber floatNumberPrefab;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private UnityEvent DeathEvent;

    public float GetCurrentHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }
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
            InstantiateFloatNumber(damageValue, Color.white);
            currentHealth -= damageValue;
            //Vector3 spawnPosition = transform.position;
            //spawnPosition.y += 1f;
            //Instantiate(damageParticle, spawnPosition, Quaternion.identity);

            SoundManager.PlayAudioClipVolume(damageClip, 1f);
            //OnDamaged?.Invoke(this, EventArgs.Empty);
        }

        Damaged();

        //if(currentHealth <= 0)
        //{
        //    CheckDeath();
        //    currentHealth = 0;
        //}
    }

    public void HeadShotDamage(float damageValue)
    {
        if (currentHealth > 0)
        {
            InstantiateFloatNumber("Head Shot!", Color.red);

            currentHealth -= damageValue;
            //Vector3 spawnPosition = transform.position;
            //spawnPosition.y += 1f;
            //Instantiate(damageParticle, spawnPosition, Quaternion.identity);

            SoundManager.PlayAudioClip(headShotClip, UnityEngine.Random.Range(0.8f,1f));
            //OnDamaged?.Invoke(this, EventArgs.Empty);
        }

        Damaged();
        //if (currentHealth <= 0)
        //{
        //    CheckDeath();
        //    currentHealth = 0;
        //}
    }

    private void Damaged()
    {
        if (currentHealth > 0)
        {
            OnDamaged?.Invoke(this, EventArgs.Empty);
        }

        if (currentHealth <= 0)
        {
            CheckDeath();
            currentHealth = 0;
        }

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 1f;

        if (damageParticle != null)
            Instantiate(damageParticle, spawnPosition, Quaternion.identity);

        if (blood != null)
            Instantiate(blood, transform.position, Quaternion.identity);
    }

    private void CheckDeath()
    {
        if(deathParticle != null)
            Instantiate(deathParticle, transform.position, Quaternion.identity);

        if (deathClip != null)
            SoundManager.PlayAudioClip(deathClip, UnityEngine.Random.Range(0.8f, 1f));

        DeathEvent?.Invoke();
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

    private void InstantiateFloatNumber(string numberValue, Color textColor)
    {
        Vector3 randomOffset = UnityEngine.Random.insideUnitCircle * 0.5f;
        Vector3 spawnPosition = transform.position + randomOffset;
        spawnPosition.y += 1f;
        FloatNumber currentFloatNumber = Instantiate(floatNumberPrefab, spawnPosition, Quaternion.identity);
        currentFloatNumber.InitFloatNumber(numberValue, textColor);
    }
}
