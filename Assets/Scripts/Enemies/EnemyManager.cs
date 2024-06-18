using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EnemyState
{
    Idle, Patrol, Chasing, Attack
}

public class EnemyManager : MonoBehaviour
{
    public EnemyState enemyState;

    private EnemyAnimations enemyAnimations;
    private EnemyMovement enemyMovement;
    private HealthController healthController;
    private Rigidbody2D rb;

    private float currentTimerHit;

    private void OnEnable()
    {
        currentTimerHit = 0.2f;

        rb = GetComponent<Rigidbody2D>();
        enemyAnimations = GetComponent<EnemyAnimations>();
        enemyMovement = GetComponent<EnemyMovement>();
        healthController = GetComponent<HealthController>();
        healthController.OnDamaged += HitMovement;
    }

    private void OnDisable()
    {
        healthController.OnDamaged -= HitMovement;
    }

    private void Update()
    {
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

    public void HitMovement(object sender, EventArgs e)
    {
        enemyAnimations.HitAnimation();
        currentTimerHit = 0f;
    }
}
