using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    public event EventHandler OnAttackEvent;
    private EnemyManager enemyManager;
    private Animator anim;

    private void Start()
    {
        enemyManager = GetComponentInParent<EnemyManager>();
        anim = GetComponent<Animator>();
        SetOverrideAnimation();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void SetOverrideAnimation()
    {
        anim.runtimeAnimatorController = enemyManager.enemySO.animOverride;
    }

    public void HitAnimation()
    {
        CrawlCheck();

        if (enemyManager.enemyState != EnemyState.Attack)
            anim.SetTrigger("Hit");
    }

    public void AttackAnimation()
    {
        anim.SetTrigger("Attack");
    }

    private void UpdateAnimator()
    {
        anim.SetFloat("Speed", enemyManager.rb.velocity.magnitude);

        if(enemyManager.enemyState == EnemyState.Death)
            anim.SetTrigger("Death");
    }

    public void BackToIdleEvent()
    {
        if (enemyManager.enemyState != EnemyState.Death)
        {
            enemyManager.rb.isKinematic = false;
            enemyManager.enemyState = EnemyState.Idle;
        }
    }

    public void AttackEvent()
    {
        OnAttackEvent?.Invoke(this, EventArgs.Empty);
    }

    private void CrawlCheck()
    {
        float currentHealth = enemyManager.healthController.GetCurrentHealth();
        float maxHealth = enemyManager.healthController.GetMaxHealth();
        if(currentHealth <= maxHealth/2 && enemyManager.enemySO.CanCrawl)
        {
            enemyManager.isCrawl = true;
            anim.SetBool("Crawl", true);
        }
    }
}
