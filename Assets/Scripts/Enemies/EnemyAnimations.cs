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
    }

    private void Update()
    {
        UpdateAnimator();
    }

    public void HitAnimation()
    {
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
            enemyManager.enemyState = EnemyState.Idle;
    }

    public void AttackEvent()
    {
        OnAttackEvent?.Invoke(this, EventArgs.Empty);
    }
}
