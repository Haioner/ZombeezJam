using UnityEngine;
using System;

[System.Serializable]
public enum EnemyState
{
    Idle, Patrol, Chasing, PreparedToAttack, Attack, Death
}

public class EnemyManager : MonoBehaviour
{
    public EnemyState enemyState;

    [HideInInspector] public HealthController healthController;
    [HideInInspector] public EnemyAnimations enemyAnimations;
    [HideInInspector] public EnemyMovement enemyMovement;
    [HideInInspector] public EnemyAttack enemyAttack;
    [HideInInspector] public Rigidbody2D rb;

    private float currentTimerHit;

    private void OnEnable()
    {
        healthController = GetComponent<HealthController>();
        enemyAnimations = GetComponentInChildren<EnemyAnimations>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody2D>();

        currentTimerHit = 0.2f;
        healthController.OnDamaged += HitMovement;
        healthController.OnDeath += Death;
    }

    private void OnDisable()
    {
        healthController.OnDamaged -= HitMovement;
        healthController.OnDeath -= Death;
    }

    private void Update()
    {
        if (enemyState == EnemyState.Death) return;

        CalculateMovementHit();
    }

    private void CalculateMovementHit()
    {
        if (currentTimerHit < 0.2f)
        {
            enemyMovement.enabled = false;
            rb.velocity = rb.velocity / 1.1f;
            currentTimerHit += Time.deltaTime;
        }
        else
        {
            enemyMovement.enabled = true;
        }
    }

    private void HitMovement(object sender, EventArgs e)
    {
        enemyAnimations.HitAnimation();
        currentTimerHit = 0f;
    }

    private void Death(object sender, EventArgs e)
    {
        enemyMovement.enabled = false;
        enemyAttack.enabled = false;
        enemyState = EnemyState.Death;
        rb.velocity = Vector2.zero;
        GetComponent<CapsuleCollider2D>().enabled = false;
        transform.GetChild(0).GetComponent<CapsuleCollider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }
}
