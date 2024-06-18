using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    public void HitAnimation()
    {
        anim.SetTrigger("Hit");
    }

    private void UpdateAnimator()
    {
        anim.SetFloat("Speed", rb.velocity.magnitude);
    }
}
