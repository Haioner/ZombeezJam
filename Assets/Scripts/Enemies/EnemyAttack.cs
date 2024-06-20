using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AttackType
{
    Meele, Ranged
}

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private AttackType attackType;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private float currentAttackCooldown;

    private EnemyManager enemyManager;
    private EnemyAnimations enemyAnimations;
    private HealthController playerHealth;

    private void OnEnable()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimations = GetComponentInChildren<EnemyAnimations>();

        enemyAnimations.OnAttackEvent += Attack;
    }

    private void OnDisable()
    {
        enemyManager.enemyAnimations.OnAttackEvent -= Attack; 
    }

    private void Update()
    {
        if (enemyManager.enemyState == EnemyState.PreparedToAttack)
        {
            CalculateAttack();
        }
    }

    private void CalculateAttack()
    {
        if (currentAttackCooldown <= attackCooldown)
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
        switch (attackType)
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
            playerHealth.TakeDamage(attackDamage);
    }

    private void AttackRanged()
    {

    }
}
