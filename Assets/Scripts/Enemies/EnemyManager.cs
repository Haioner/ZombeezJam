using UnityEngine;
using System;
using DG.Tweening;

[Serializable]
public enum EnemyState
{
    Idle, Patrol, Chasing, PreparedToAttack, Attack, Death
}

public class EnemyManager : MonoBehaviour
{
    public EnemySO enemySO;
    [SerializeField] private LayerMask detectLayer;

    [Header("Fade")]
    [SerializeField] private DOTweenAnimation fadeGFXDOT; 
    [SerializeField] private DOTweenAnimation fadeShadowDOT; 

    [Header("Roar")]
    [SerializeField] private AudioClip roarClip;
    private bool canRoar = true;
    private float lastRoarTime;
    private float roarCooldown = 5f;

    [Header("Colliders")]
    [SerializeField] private GameObject standColliders;
    [SerializeField] private GameObject crawlColliders;

    public EnemyState enemyState;
    [HideInInspector] public HealthController healthController;
    [HideInInspector] public EnemyAnimations enemyAnimations;
    [HideInInspector] public EnemyMovement enemyMovement;
    [HideInInspector] public EnemyAttack enemyAttack;
    [HideInInspector] public Rigidbody2D rb;
    public Transform player { get; set; }
    [HideInInspector] public bool isCrawl;

    private ItemDropper itemDropper;
    private float currentTimerStopMovementHit;
    private RoomController currentRoom;

    private void Awake()
    {
        itemDropper = GetComponent<ItemDropper>();
        healthController = GetComponent<HealthController>();
        enemyAnimations = GetComponentInChildren<EnemyAnimations>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody2D>();
        healthController.SetNewMaxHealth(enemySO.MaxHealth);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        currentTimerStopMovementHit = 0.2f;
        healthController.OnDamaged += StopMovementHit;
        healthController.OnDeath += Death;
    }

    private void OnDisable()
    {
        healthController.OnDamaged -= StopMovementHit;
        healthController.OnDeath -= Death;
    }

    private void Update()
    {
        if (enemyState == EnemyState.Death) return;

        StopMovementHitTimer();
        CheckRoar();
    }

    public void InitiateRandomEnemy(EnemySO newEnemySO, RoomController room)
    {
        enemySO = newEnemySO;
        currentRoom = room;

        itemDropper.drops = enemySO.drops;
    }

    public float GetPlayerDistance()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    public bool SawPlayer()
    {
        Vector3 playerPos = Vector3.zero;
        if(player != null)
            playerPos = player.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerPos - transform.position).normalized, Mathf.Infinity, detectLayer);
        return hit.collider.CompareTag("Player");
    }

    private void CheckRoar()
    {
        if (SawPlayer() && canRoar && GetPlayerDistance() > 1 && GetPlayerDistance() < 20)
        {
            canRoar = false;
            SoundManager.PlayAudioClip(roarClip);
            lastRoarTime = Time.time;
        }
        else if (!SawPlayer() && !canRoar)
        {
            if (!canRoar && Time.time - lastRoarTime > roarCooldown)
            {
                canRoar = true;
            }
        }
    }

    private void StopMovementHitTimer()
    {
        if (currentTimerStopMovementHit < 0.2f)
        {
            enemyMovement.enabled = false;
            rb.velocity = rb.velocity / 1.1f;
            currentTimerStopMovementHit += Time.deltaTime;
        }
        else
        {
            enemyMovement.enabled = true;
        }
    }

    private void StopMovementHit(object sender, EventArgs e)
    {
        enemyAnimations.HitAnimation();
        currentTimerStopMovementHit = 0f;
        SwitchColliders();
    }

    private void SwitchColliders()
    {
        if (isCrawl)
        {
            standColliders.SetActive(false);
            if (crawlColliders != null)
                crawlColliders.SetActive(true);
        }
    }

    private void Death(object sender, EventArgs e)
    {
        GetComponent<ItemDropper>().DropCollectables();
        fadeGFXDOT.DORestart();
        fadeShadowDOT.DORestart();
        enemyMovement.enabled = false;
        enemyAttack.enabled = false;
        enemyState = EnemyState.Death;
        rb.velocity = Vector2.zero;
        standColliders.SetActive(false);
        if (crawlColliders != null)
            crawlColliders.SetActive(false);

        transform.GetComponent<CapsuleCollider2D>().enabled = false;
        if (currentRoom != null)
            currentRoom.RemoveEnemyFromList(this);
        Destroy(gameObject, 2f);
    }
}
