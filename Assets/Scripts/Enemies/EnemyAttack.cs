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
        if (distance <= enemyManager.enemySO.AttackRange)
            playerHealth.TakeDamage(enemyManager.enemySO.AttackDamage);
    }

    private void AttackRanged()
    {
        if (playerHealth == null)
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();

        Vector2 targetPosition = playerHealth.transform.position;
        targetPosition.y += 0.8f;

        Vector2 directionToPlayer = (targetPosition - (Vector2)attackPivot.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        SpawnProjectile(angle);

        if (enemyManager.enemySO.fireParticleAttack != null)
            SpawnShootFire(angle);
    }

    private void SpawnProjectile(float angle)
    {
        Projectile newBullet = Instantiate(enemyManager.enemySO.AttackProjectile, attackPivot.position, Quaternion.identity);
        newBullet.InitProjectile(angle, enemyManager.enemySO.AttackDamage, gameObject);
    }

    private void SpawnShootFire(float angle)
    {
        ParticleSystem fire = Instantiate(enemyManager.enemySO.fireParticleAttack, attackPivot.position, Quaternion.identity);
        var main = fire.main;
        main.startRotation = -angle * Mathf.Deg2Rad;

        Vector3 fireDirection = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        fire.transform.rotation = Quaternion.LookRotation(Vector3.forward, fireDirection);
    }
}
