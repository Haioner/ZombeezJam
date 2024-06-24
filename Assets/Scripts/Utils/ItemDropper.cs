using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drops
{
    public GameObject dropPrefab;
    public float DropChance = 30f;
}

public class ItemDropper : MonoBehaviour
{
    public List<Drops> drops = new List<Drops>();
    private bool hasDropped;

    public void DropCollectables()
    {
        if (hasDropped) return;

        hasDropped = true;
        float totalDropChance = 0f;

        foreach (var drop in drops)
        {
            totalDropChance += drop.DropChance;
        }

        float randomValue = Random.Range(0f, 100f);

        if (randomValue > totalDropChance)
        {
            return;
        }

        float cumulativeChance = 0f;

        foreach (var drop in drops)
        {
            cumulativeChance += drop.DropChance;
            if (randomValue <= cumulativeChance)
            {
                Instantiate(drop.dropPrefab, transform.position, Quaternion.identity);
                break;
            }
        }
    }

    private bool CanDrop(float dropChance)
    {
        float randomValue = Random.value;
        return randomValue < (dropChance / 100) ? true : false;
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
