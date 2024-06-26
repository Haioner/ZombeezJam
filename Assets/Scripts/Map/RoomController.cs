using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomSettings
{
    [Header("Room")]
    public RoomListSO RoomListSO;
    public GameObject SafeRoom;
    public PolygonCollider2D PolygonSpawnArea;

    [Header("Room Type")]
    public Transform groundParent;
    public Sprite[] randGrounds;
    public Tilemap[] wallTileMap;
    public Color[] randWallColor;

    [Header("End")]
    public Transform EndPoint;
    public ParticleSystem ClosedParticle;
    public SpriteRenderer EndLightSprite;
    public Light2D EndLight;
}

[System.Serializable]
public class CollectableSpawner
{
    public Transform collectableHolder;
    public List<GameObject> collectablesPrefabs = new List<GameObject>();
    public Vector2 collectableCountRange = new Vector2(1, 5);
}

[System.Serializable]
public class EnemySpawner
{
    public EnemyManager EnemyPrefab;
    public Vector2 EnemyCountRange = new Vector2(1, 5);
    public EnemiesListSO RoomEnemiesList;
}

public class RoomController : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private RoomSettings roomSettings;
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
        RandRoomType();
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
            roomSettings.EndPoint.GetComponentInChildren<DOTweenAnimation>().DORestart();
            if (roomSettings.ClosedParticle != null)
            {
                var emission = roomSettings.ClosedParticle.emission;
                emission.rateOverTime = 0;
            }
            GreenEndLight();
        }
    }

    public void GreenEndLight()
    {
        if (roomSettings.EndLight == null) return;

        roomSettings.EndLightSprite.color = Color.green;
        roomSettings.EndLight.color = Color.green;
    }

    public void RedEndLight()
    {
        if (roomSettings.EndLight == null) return;

        roomSettings.EndLightSprite.color = Color.red;
        roomSettings.EndLight.color = Color.red;
    }

    private void SpawnNewRoom()
    {
        if (GameManager.instance.CurrentRoom % 10 == 0)
        {
            Instantiate(roomSettings.SafeRoom, roomSettings.EndPoint.position, Quaternion.identity);
        }
        else
        {
            int maxRoom = GameManager.instance.CurrentRoom + 2 < roomSettings.RoomListSO.rooms.Count ? 
                GameManager.instance.CurrentRoom + 2 : roomSettings.RoomListSO.rooms.Count;
            int randRoom = Random.Range(0, maxRoom);
            Instantiate(roomSettings.RoomListSO.rooms[randRoom], roomSettings.EndPoint.position, Quaternion.identity);
        }
    }

    private void RandRoomType()
    {
        int randGround = Random.Range(0, roomSettings.randGrounds.Length);
        foreach (Transform child in roomSettings.groundParent.transform)
        {
            child.GetComponent<SpriteRenderer>().sprite = roomSettings.randGrounds[randGround];
        }

        int randWallColor = Random.Range(0, roomSettings.randWallColor.Length);
        foreach (var tileMap in roomSettings.wallTileMap)
        {
            tileMap.color = roomSettings.randWallColor[randWallColor];
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

        //Increase enemy count per room after min 10 room and max 30 room
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

                //Increase Rand Enemy Type by room per 5 rooms
                int maxRandEnemy = Mathf.Min(GameManager.instance.CurrentRoom / 5, enemySpawner.RoomEnemiesList.EnemiesList.Count - 1);
                int randEnemy = Random.Range(0, maxRandEnemy + 1);
                newEnemy.InitiateRandomEnemy(enemySpawner.RoomEnemiesList.EnemiesList[randEnemy], this);

                //Health increase by room
                float chanceRoom = GameManager.instance.CurrentRoom / 30f;
                float healthChance = GameManager.instance.CurrentRoom < 15 ? 1 : Random.Range(0f, 1f);
                if (healthChance <= chanceRoom)
                {
                    if (GameManager.instance.CurrentRoom != 0)
                    {
                        float newMaxHealth = newEnemy.enemySO.MaxHealth + (GameManager.instance.CurrentRoom * 15f);
                        newEnemy.healthController.SetNewMaxHealth(newMaxHealth);
                    }
                }
            }
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        int spawnAreaLayerMask = 1 << LayerMask.NameToLayer("SpawnArea");

        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomPosition = GetRandomPointInPolygon(roomSettings.PolygonSpawnArea);

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
