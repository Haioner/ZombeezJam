using UnityEngine;
using System;

public class EnemyAttack : MonoBehaviour
{
    [Header("Ranged")]
    [SerializeField] private Transform attackPivot;

    private float currentAttackCooldown;
    private EnemyManager enemyManager;
    private EnemyAnimations enemyAnimations;
    private HealthController playerHealth;

    private void OnEnable()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimations = GetComponentInChildren<EnemyAnimations>();

        enemyAnimations.OnAttackEvent += Attack;

        currentAttackCooldown = enemyManager.enemySO.AttackDamage;
    }

    private void OnDisable()
    {
        enemyManager.enemyAnimations.OnAttackEvent -= Attack; 
    }

    private void Update()
    {
        if (enemyManager.enemyState == EnemyState.PreparedToAttack)
            CalculateAttack();
    }

    private void CalculateAttack()
    {
        if (currentAttackCooldown <= enemyManager.enemySO.AttackCooldown)
            currentAttackCooldown += Time.deltaTime;
        else
        {
            currentAttackCooldown = 0f;
            enemyManager.enemyState = EnemyState.Attack;
            enemyAnimations.AttackAnimation();
        }
    }

    private void Attack(object sender, EventArgs e)
    {
        SoundManager.PlayAudioClip(enemyManager.enemySO.AttackClip, UnityEngine.Random.Range(0.9f,1.2f));
        switch (enemyManager.enemySO.attackType)
        {
            case AttackType.Meele: AttackMeele(); break;
            case AttackType.Ranged: AttackRanged(); break;
        }
    }

    private void AttackMeele()
    {
        if(playerHealth == null)
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();

        float distance = Vector2.Distance(playerHealth.transform.position, transform.position);
        if (distance <= 1.1f)
            playerHealth.TakeDamage(enemyManager.enemySO.AttackDamage);
    }

    private void AttackRanged()
    {
        if (playerHealth == null)
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();

        Vector2 directionToPlayer = (playerHealth.transform.position - attackPivot.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        SpawnProjectile(angle);
    }

    private void SpawnProjectile(float angle)
    {
        Projectile newBullet = Instantiate(enemyManager.enemySO.AttackProjectile, attackPivot.position, Quaternion.identity);
        newBullet.InitProjectile(angle, enemyManager.enemySO.AttackDamage, gameObject);
    }
}
