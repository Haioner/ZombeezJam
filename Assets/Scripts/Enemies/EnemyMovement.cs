using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distanceToStop = 1f;
    [SerializeField] private Transform flipable;
    private Vector2 lastKnownTargetPosition;

    [Header("Detection Range")]
    [SerializeField] private float detectionShootRange = 10f;
    [SerializeField] private float chaseShootTimer = 3f;
    private float currentChaseShootTimer;
    private bool isChasingShoot;

    [Header("Patrol")]
    [SerializeField] private float patrolCooldown = 2f;
    [SerializeField] private float minDistancePatrol = 5f;
    [SerializeField] private float maxDistancePatrol = 15f;
    private float currentPatrolCooldown;

    [Header("Avoid Obstacles")]
    [SerializeField] private int numRays = 10;
    [SerializeField] private float avoidForceMultiplier = 25f;
    [SerializeField] private float avoidConeAngle = 180f;
    [SerializeField] private float avoidDetectionDistance = 1.12f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool canRotate;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask detectLayer;

    private EnemyManager enemyManager;
    private Transform player;
    private Rigidbody2D rb;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        lastKnownTargetPosition = rb.position;
    }

    private void Update()
    {
        if (player == null) return;
        DetectShoot();
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        DetectPlayer();
        UpdatePatrol();

        Vector2 moveDirection = AvoidWalls((lastKnownTargetPosition - rb.position).normalized);
        MoveTowards(moveDirection);
    }

    private void DetectShoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Vector2.Distance(transform.position, player.position) <= detectionShootRange)
            {
                lastKnownTargetPosition = player.position;
                enemyManager.enemyState = EnemyState.Chasing;

                isChasingShoot = true;
                currentChaseShootTimer = 0f;
            }
        }

        if (isChasingShoot)
        {
            currentChaseShootTimer += Time.deltaTime;
            if (currentChaseShootTimer >= chaseShootTimer)
            {
                lastKnownTargetPosition = rb.position;
                enemyManager.enemyState = EnemyState.Idle;
                isChasingShoot = false;
            }
        }
    }

    private void DetectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, Mathf.Infinity, detectLayer);
        float distance = Vector2.Distance(lastKnownTargetPosition, rb.position);
        if (hit.collider.CompareTag("Player"))
        {
            enemyManager.enemyState = EnemyState.Chasing;
            lastKnownTargetPosition = player.position;

            if (distance <= distanceToStop)
                enemyManager.enemyState = EnemyState.Attack;
        }
        else if (distance <= distanceToStop)
        {
            enemyManager.enemyState = EnemyState.Idle;
        }
    }

    private void UpdatePatrol()
    {
        if (enemyManager.enemyState == EnemyState.Chasing) return;

        if (currentPatrolCooldown < patrolCooldown && enemyManager.enemyState == EnemyState.Idle)
        {
            currentPatrolCooldown += Time.deltaTime;
        }

        if (currentPatrolCooldown >= patrolCooldown)
        {
            currentPatrolCooldown = 0;
            enemyManager.enemyState = EnemyState.Patrol;

            Vector2 randomPosition = FindRandomPositionWithoutWall();
            lastKnownTargetPosition = randomPosition;
        }
    }

    private Vector2 FindRandomPositionWithoutWall()
    {
        Vector2 randomPosition = Vector2.zero;
        bool foundValidPosition = false;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            randomPosition = (Vector2)transform.position + direction * Random.Range(minDistancePatrol, maxDistancePatrol);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistancePatrol, wallLayerMask);

            if (hit.collider != null)
            {
                randomPosition = hit.point - direction * 0.314f;
                foundValidPosition = true;
                break;
            }
            else
            {
                foundValidPosition = true;
                break;
            }
        }

        if (!foundValidPosition)
        {
            Debug.LogWarning("Unable to find valid random position without colliding with walls. Using fallback position.");
            randomPosition = (Vector2)transform.position;
        }

        return randomPosition;
    }

    private void MoveTowards(Vector2 direction)
    {
        float distance = Vector2.Distance(lastKnownTargetPosition, rb.position);
        if (distance > distanceToStop)
        {
            rb.velocity = direction * speed;

            if (canRotate)
            {
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float smoothedAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime);
                rb.rotation = smoothedAngle;
            }
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop the movement when within the stopping distance
        }
        Flip();
    }

    private void Flip()
    {
        if (lastKnownTargetPosition == (Vector2)transform.position) return;

        Vector3 localScale = flipable.localScale;
        localScale.x = lastKnownTargetPosition.x < transform.position.x ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        flipable.localScale = localScale;
    }

    private Vector2 AvoidWalls(Vector2 direction)
    {
        float angleStep = avoidConeAngle / (numRays - 1);
        Vector2 avoidForce = Vector2.zero;
        Vector2 avoidDirection = Vector2.zero;
        float closestHitDistance = float.MaxValue;
        int numHits = 0;

        for (int i = 0; i < numRays; i++)
        {
            Vector2 rayDirection = Quaternion.Euler(0, 0, -(avoidConeAngle / 2) + (angleStep * i)) * direction;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, avoidDetectionDistance, wallLayerMask);

            if (hit.collider != null)
            {
                numHits++;
                float distanceToHit = hit.distance;
                float forceMultiplier = Mathf.Lerp(1f, 2f, Mathf.Abs(i - (numRays / 2f)) / (numRays / 2f));
                if (distanceToHit < closestHitDistance)
                {
                    closestHitDistance = distanceToHit;
                    avoidDirection = -rayDirection.normalized;
                }
                avoidForce += rayDirection.normalized * avoidForceMultiplier * forceMultiplier;
            }
        }

        if (closestHitDistance < float.MaxValue)
        {
            avoidForce = avoidDirection * avoidForceMultiplier * (1f - (closestHitDistance / avoidDetectionDistance));
        }

        float strengthRatio = Mathf.Lerp(1f, 2f, 1f - ((float)numHits / numRays));
        return (direction + avoidForce * strengthRatio).normalized;
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, lastKnownTargetPosition);

        Vector2 direction = (lastKnownTargetPosition - (Vector2)transform.position).normalized;
        float angleStep = avoidConeAngle / (numRays - 1);

        Gizmos.color = Color.red;
        for (int i = 0; i < numRays; i++)
        {
            Vector2 rayDirection = Quaternion.Euler(0, 0, -(avoidConeAngle / 2) + (angleStep * i)) * direction;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, avoidDetectionDistance, wallLayerMask);
            if (hit.collider != null)
            {
                Gizmos.DrawRay(transform.position, rayDirection * hit.distance);
                Gizmos.DrawSphere(hit.point, 0.1f);
            }
            else
            {
                Gizmos.DrawRay(transform.position, rayDirection * avoidDetectionDistance);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceToStop);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionShootRange);
    }
}
