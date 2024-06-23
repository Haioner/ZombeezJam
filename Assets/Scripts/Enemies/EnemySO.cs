using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AttackType
{
    Meele, Ranged
}

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Animation")]
    public AnimatorOverrideController animOverride;

    [Header("Movement")]
    public float Speed = 2f;
    public float CrawlSpeed = 2f;
    public float DistanceToStop = 1.1f;
    public bool CanCrawl = true;

    [Header("Attack")]
    public AttackType attackType;
    public float AttackDamage = 10f;
    public float AttackCooldown = 5f;
    public float AttackRange = 5f;
    public AudioClip AttackClip;
    public Projectile AttackProjectile;

    [Header("Drops")]
    public List<Drops> drops = new List<Drops>();
}
