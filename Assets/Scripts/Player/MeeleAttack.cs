using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPivot;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float staminaAmount = 10f;
    [SerializeField] private GameObject slashParticle;
    private float currentAttackCooldown;
    private PlayerManager playerManager;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CalculateCooldown();

        if (Input.GetMouseButton(1) && currentAttackCooldown <= 0 && playerMovement.GetCurrentStamina() > staminaAmount)
            Attack();
    }

    private void CalculateCooldown()
    {
        if (currentAttackCooldown > 0)
            currentAttackCooldown -= Time.deltaTime;
    }

    private void Attack()
    {
        playerMovement.SubtractCurrentStamina(staminaAmount);
        currentAttackCooldown = attackCooldown;

        GameObject slash = Instantiate(slashParticle, attackPivot);
        slash.transform.SetParent(null);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPivot.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            HealthController healthController = hitCollider.GetComponent<HealthController>();
            if (healthController != null && hitCollider.gameObject != gameObject)
            {
                healthController.TakeDamage(playerManager.MeeleDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPivot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPivot.position, attackRadius);
        }
    }
}
