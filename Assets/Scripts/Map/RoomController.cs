using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawner
{
    public EnemyManager EnemyPrefab;
    public Vector2 EnemyCountRange = new Vector2(1, 5);
    public List<EnemySO> RoomEnemiesType = new List<EnemySO>();
}

[System.Serializable]
public class CollectableSpawner
{
    public Transform collectableHolder;
    public List<GameObject> collectablesPrefabs = new List<GameObject>();
    public Vector2 collectableCountRange = new Vector2(1, 5);
}

public class RoomController : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private Transform endPoint;
    [SerializeField] private PolygonCollider2D spawnArea;
    [SerializeField] private RoomListSO roomListSO;
    [SerializeField] private GameObject safeRoom;
    [SerializeField] private ParticleSystem closedParticle;
    private bool hasSpawnedRoom;

    [Header("Collectables")]
    [SerializeField] private bool canSpawnCollectable = true;
    [SerializeField] private CollectableSpawner collectableSpawner;

    [Header("Enemy")]
    [SerializeField] private bool canSpawnEnemy = true;
    [SerializeField] private EnemySpawner enemySpawner;
    private List<EnemyManager> currentEnemiesList = new List<EnemyManager>();

    private void OnEnable()
    {
        SpawnRandomizedCollectables();
        SpawnRandomizedEnemies();
    }

    public void NextRoom()
    {
        if (currentEnemiesList.Count > 0) return;

        if (!hasSpawnedRoom)
        {
            hasSpawnedRoom = true;
            GameManager.instance.AddCurrentRoom();
            SpawnNewRoom();
        }
    }

    private void SpawnNewRoom()
    {
        if (GameManager.instance.CurrentRoom % 10 == 0)
        {
            Instantiate(safeRoom, endPoint.position, Quaternion.identity);
        }
        else
        {
            int maxRoom = GameManager.instance.CurrentRoom + 2 < roomListSO.rooms.Count ? GameManager.instance.CurrentRoom + 2 : roomListSO.rooms.Count;
            int minRoom = GameManager.instance.CurrentRoom - 1 < maxRoom && maxRoom - 4 >= 0 ? maxRoom - 4 : 0;
            int randRoom = Random.Range(minRoom, maxRoom);

            Instantiate(roomListSO.rooms[randRoom], endPoint.position, Quaternion.identity);
        }
    }

    private void SpawnRandomizedCollectables()
    {
        if (!canSpawnCollectable) return;

        int spawnCollectableCount = (int)Random.Range(collectableSpawner.collectableCountRange.x, collectableSpawner.collectableCountRange.y);

        for (int i = 0; i < spawnCollectableCount; i++)
        {
            Vector2 spawnPosition = GetValidSpawnPosition();
            if (spawnPosition != Vector2.zero)
            {
                int randCollectable = Random.Range(0, collectableSpawner.collectablesPrefabs.Count);
                GameObject collectable = Instantiate(collectableSpawner.collectablesPrefabs[randCollectable], spawnPosition, Quaternion.identity);
                collectable.transform.SetParent(collectableSpawner.collectableHolder);
            }
        }
    }

    private void SpawnRandomizedEnemies()
    {
        if (!canSpawnEnemy) return;

        int minEnemySpawn = enemySpawner.EnemyCountRange.x + GameManager.instance.CurrentRoom > 10 ?
            10 : (int)enemySpawner.EnemyCountRange.x + GameManager.instance.CurrentRoom;

        int maxEnemySpawn = enemySpawner.EnemyCountRange.y + GameManager.instance.CurrentRoom > 30 ?
            30 : (int)enemySpawner.EnemyCountRange.y + GameManager.instance.CurrentRoom;

        int spawnCount = Random.Range(minEnemySpawn, maxEnemySpawn);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPosition = GetValidSpawnPosition();
            if (spawnPosition != Vector2.zero)
            {
                EnemyManager newEnemy = Instantiate(enemySpawner.EnemyPrefab, spawnPosition, Quaternion.identity);
                currentEnemiesList.Add(newEnemy);
                int randEnemy = Random.Range(0, enemySpawner.RoomEnemiesType.Count);
                newEnemy.InitiateRandomEnemy(enemySpawner.RoomEnemiesType[randEnemy], this);

                if (Random.value < 0.5f)
                {
                    if (GameManager.instance.CurrentRoom != 0)
                    {
                        float newMaxHealth = newEnemy.healthController.GetMaxHealth() + (GameManager.instance.CurrentRoom * 2f);
                        newEnemy.healthController.SetNewMaxHealth(newMaxHealth);
                    }
                }
            }
        }
    }

    public void RemoveEnemyFromList(EnemyManager enemyManager)
    {
        currentEnemiesList.Remove(enemyManager);
        CheckCanNextRoom();
    }

    private void CheckCanNextRoom()
    {
        if (currentEnemiesList.Count <= 0)
        {
            if (GameManager.instance != null)
                GameManager.instance.CleanText();

            NextRoom();
            endPoint.GetComponentInChildren<DOTweenAnimation>().DORestart();
            if (closedParticle != null)
            {
                var emission = closedParticle.emission;
                emission.rateOverTime = 0;
            }
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        int spawnAreaLayerMask = 1 << LayerMask.NameToLayer("SpawnArea");

        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomPosition = GetRandomPointInPolygon(spawnArea);

            Collider2D hitCollider = Physics2D.OverlapCircle(randomPosition, 1.2f, ~spawnAreaLayerMask);
            if (hitCollider == null)
            {
                return randomPosition;
            }
        }
        return Vector2.zero;
    }

    private Vector2 GetRandomPointInPolygon(PolygonCollider2D polygon)
    {
        // Get bounds of the polygon
        Bounds bounds = polygon.bounds;
        Vector2 randomPoint = Vector2.zero;

        // Check if the point is inside the polygon
        for (int attempt = 0; attempt < 100; attempt++)
        {
            randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (IsPointInPolygon(polygon, randomPoint))
            {
                return randomPoint;
            }
        }

        return randomPoint;
    }

    private bool IsPointInPolygon(PolygonCollider2D polygon, Vector2 point)
    {
        bool isInside = false;
        int j = polygon.points.Length - 1;
        for (int i = 0; i < polygon.points.Length; j = i++)
        {
            Vector2 p1 = polygon.transform.TransformPoint(polygon.points[i]);
            Vector2 p2 = polygon.transform.TransformPoint(polygon.points[j]);

            if (((p1.y <= point.y && point.y < p2.y) || (p2.y <= point.y && point.y < p1.y)) &&
                (point.x < (p2.x - p1.x) * (point.y - p1.y) / (p2.y - p1.y) + p1.x))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }
}
