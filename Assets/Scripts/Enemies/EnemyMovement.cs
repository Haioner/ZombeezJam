using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Flip")]
    [SerializeField] private Transform flipable;

    [Header("Avoid Obstacles")]
    [SerializeField] private int numRays = 6;
    [SerializeField] private float avoidForceMultiplier = 1.4f;
    [SerializeField] private float avoidConeAngle = 100f;
    [SerializeField] private float avoidDetectionDistance = 0.9f;

    [Header("Shoot Detection Range")]
    [SerializeField] private float shootDetectionRange = 10f;
    [SerializeField] private float chaseShootTimer = 8f;
    private float currentChaseShootTimer;
    private bool isChasingShoot;

    [Header("Patrol")]
    [SerializeField] private float patrolCooldown = 1f;
    [SerializeField] private float minDistancePatrol = 2.5f;
    [SerializeField] private float maxDistancePatrol = 10f;
    private float currentPatrolCooldown;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool canRotate;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask wallLayerMask;

    //CACHE
    private Vector2 recordedPerpendicularDirection;
    private Vector2 lastKnownTargetPosition;
    private Vector2 firstHitDirection;
    private bool isAvoidingWall = false;
    private bool allRaysHittingWall = false;
    private float stopaAvoidWallCooldown = 0.2f;
    private float stopAvoidWallTimer = 0f;

    private WeaponController weaponController;
    private EnemyManager enemyManager;
    private Rigidbody2D rb;

    #region Methods
    private void OnEnable()
    {
        weaponController = FindObjectOfType<WeaponController>();
        enemyManager = GetComponent<EnemyManager>();
        rb = GetComponent<Rigidbody2D>();

        weaponController.OnShoot += DetectOnShoot;

        lastKnownTargetPosition = rb.position;
    }

    private void OnDisable()
    {
        weaponController.OnShoot -= DetectOnShoot;
    }

    private void Update()
    {
        if (enemyManager.player == null || enemyManager.enemyState == EnemyState.Attack) return;

        ShootDetectionTimer();
    }

    private void FixedUpdate()
    {
        if (enemyManager.player == null || enemyManager.enemyState == EnemyState.Attack) return;

        CalculateTarget();
        UpdatePatrol();

        Vector2 moveDirection = AvoidWalls((lastKnownTargetPosition - rb.position).normalized);
        MoveTowards(moveDirection);
        Flip(moveDirection);
    }
    #endregion

    #region Move
    private float GetCurrentSpeed()
    {
        return enemyManager.isCrawl ? enemyManager.enemySO.CrawlSpeed : enemyManager.enemySO.Speed;
    }

    private void MoveTowards(Vector2 direction)
    {
        float distanceTarget = Vector2.Distance(lastKnownTargetPosition, rb.position);
        float distanceToStop = enemyManager.SawPlayer() ? enemyManager.enemySO.AttackRange : enemyManager.enemySO.DistanceToStop;

        if (distanceTarget > distanceToStop)
        {
            //rb.velocity = direction * enemyManager.enemySO.Speed;
            rb.velocity = direction * GetCurrentSpeed();

            if (canRotate)
            {
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float smoothedAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime);
                rb.rotation = smoothedAngle;
            }
        }
        else
            rb.velocity = Vector2.zero; 
    }

    private void Flip(Vector2 moveDirection)
    {
        if (moveDirection.x < 0)
            flipable.localScale = new Vector3(Mathf.Abs(flipable.localScale.x), flipable.localScale.y, flipable.localScale.z);
        else if (moveDirection.x > 0)
            flipable.localScale = new Vector3(-Mathf.Abs(flipable.localScale.x), flipable.localScale.y, flipable.localScale.z);
    }
    #endregion

    #region Shoot Detection
    private void DetectOnShoot(object sender, System.EventArgs e)
    {
        if (enemyManager.GetPlayerDistance() <= shootDetectionRange)
        {
            lastKnownTargetPosition = enemyManager.player.position;

            isChasingShoot = true;
            currentChaseShootTimer = 0f;

            ResetAvoidance();
        }
    }

    private void ShootDetectionTimer()
    {
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
    #endregion

    #region Detect Player
    private void CalculateTarget()
    {
        float distanceToTarget = Vector2.Distance(lastKnownTargetPosition, rb.position);
        if (enemyManager.SawPlayer())
        {
            enemyManager.enemyState = EnemyState.Chasing;
            lastKnownTargetPosition = enemyManager.player.position;

            //Prepared to attack
            if (enemyManager.GetPlayerDistance() <= enemyManager.enemySO.AttackRange)
                enemyManager.enemyState = EnemyState.PreparedToAttack;
        }
        else if (distanceToTarget <= enemyManager.enemySO.DistanceToStop)
        {
            enemyManager.enemyState = EnemyState.Idle;
        }
    }
    #endregion

    #region Patrol
    private void UpdatePatrol()
    {
        if (enemyManager.enemyState != EnemyState.Idle) return;

        if (currentPatrolCooldown < patrolCooldown)
            currentPatrolCooldown += Time.deltaTime;

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
    #endregion

    #region Avoid Wall
    private Vector2 AvoidWalls(Vector2 direction)
    {
        float angleStep = avoidConeAngle / (numRays - 1);
        float closestHitDistance = float.MaxValue;
        Vector2 avoidDirection = Vector2.zero;
        Vector2 avoidForce = Vector2.zero;
        int numHits = 0;

        //Calculate rays cone
        for (int i = 0; i < numRays; i++)
        {
            Vector2 rayDirection = Quaternion.Euler(0, 0, -(avoidConeAngle / 2) + (angleStep * i)) * direction;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, avoidDetectionDistance, wallLayerMask);

            if (hit.collider != null)
            {
                numHits++;
                float distanceToHit = hit.distance;
                float forceMultiplier = Mathf.Lerp(1f, 2f, Mathf.Abs(i - (numRays / 2f)) / (numRays / 2f));

                if (!allRaysHittingWall)
                {
                    firstHitDirection = rayDirection.normalized;
                    allRaysHittingWall = true;
                }

                if (distanceToHit < closestHitDistance)
                {
                    closestHitDistance = distanceToHit;
                    avoidDirection = -rayDirection.normalized;
                }
                avoidForce += rayDirection.normalized * avoidForceMultiplier * forceMultiplier;
            }
        }

        float hitPercentage = (float)numHits / numRays;
        if (hitPercentage >= 0.8f) //80% hit wall
        {
            if (!isAvoidingWall)
            {
                isAvoidingWall = true;

                // Determine perpendicular direction based on the direction of the first hit ray
                Vector2 directionStart = Quaternion.Euler(0, 0, avoidConeAngle / 2) * firstHitDirection;
                Vector2 directionEnd = Quaternion.Euler(0, 0, -avoidConeAngle / 2) * firstHitDirection;

                float angleToStart = Vector2.Angle(direction, directionStart);
                float angleToEnd = Vector2.Angle(direction, directionEnd);

                if (recordedPerpendicularDirection == Vector2.zero)
                {
                    if (angleToStart < angleToEnd)
                        recordedPerpendicularDirection = new Vector2(-firstHitDirection.y, firstHitDirection.x).normalized;
                    else
                        recordedPerpendicularDirection = new Vector2(firstHitDirection.y, -firstHitDirection.x).normalized;
                }
            }
            stopAvoidWallTimer = 0f;
        }
        else if(hitPercentage <= 0.3f) //30% or minus
        {
            stopAvoidWallTimer += Time.deltaTime;
            if (stopAvoidWallTimer >= stopaAvoidWallCooldown)
            {
                allRaysHittingWall = false;
                isAvoidingWall = false;
                recordedPerpendicularDirection = Vector2.zero;
            }

        }

        if (isAvoidingWall)
        {
            // Apply a stronger perpendicular force to the recorded perpendicular direction
            avoidForce = recordedPerpendicularDirection * avoidForceMultiplier * 3f;
        }
        else if (closestHitDistance < float.MaxValue)
        {
            avoidForce = avoidDirection * avoidForceMultiplier * (1f - (closestHitDistance / avoidDetectionDistance));
        }

        float strengthRatio = Mathf.Lerp(1f, 2f, 1f - ((float)numHits / numRays));
        return (direction + avoidForce * strengthRatio).normalized;
    }

    private void ResetAvoidance()
    {
        allRaysHittingWall = false;
        isAvoidingWall = false;

        float closestDistanceToCenter = float.MaxValue;
        Vector2 closestRayDirection = Vector2.zero;

        for (int i = 0; i < numRays; i++)
        {
            float angle = -(avoidConeAngle / 2) + (avoidConeAngle * i / (numRays - 1));
            Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * (lastKnownTargetPosition - rb.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, avoidDetectionDistance, wallLayerMask);
            if (hit.collider != null)
            {
                float distanceToCenter = Vector2.Distance(transform.position, hit.point);

                if (distanceToCenter < closestDistanceToCenter)
                {
                    closestDistanceToCenter = distanceToCenter;
                    closestRayDirection = rayDirection.normalized;
                }
            }
        }

        //Calculate perpendicular direction based on closest ray
        if (closestDistanceToCenter < float.MaxValue)
        {
            Vector2 directionStart = Quaternion.Euler(0, 0, avoidConeAngle / 2) * closestRayDirection;
            Vector2 directionEnd = Quaternion.Euler(0, 0, -avoidConeAngle / 2) * closestRayDirection;

            float angleToStart = Vector2.Angle((lastKnownTargetPosition - rb.position).normalized, directionStart);
            float angleToEnd = Vector2.Angle((lastKnownTargetPosition - rb.position).normalized, directionEnd);

            if (angleToStart < angleToEnd)
                recordedPerpendicularDirection = new Vector2(-closestRayDirection.y, closestRayDirection.x).normalized;
            else
                recordedPerpendicularDirection = new Vector2(closestRayDirection.y, -closestRayDirection.x).normalized;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (enemyManager == null || enemyManager.player == null) return;

        //Cone rays wall avoidence
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

        Gizmos.color = Color.green; //Ray to target
        Gizmos.DrawLine(transform.position, lastKnownTargetPosition);

        Gizmos.color = Color.cyan; //Perpendicular ray direction
        Gizmos.DrawRay(transform.position, recordedPerpendicularDirection * avoidDetectionDistance);

        Gizmos.color = Color.yellow; //Circle stop distance
        Gizmos.DrawWireSphere(transform.position, enemyManager.enemySO.DistanceToStop);

        Gizmos.color = Color.blue; //Circle shoot detection
        Gizmos.DrawWireSphere(transform.position, shootDetectionRange);
    }
}
