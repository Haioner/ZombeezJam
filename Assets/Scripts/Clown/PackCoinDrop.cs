using System.Collections;
using UnityEngine;

public class PackCoinDrop : MonoBehaviour
{
    [SerializeField] private CoinDrop coinDrop;
    [SerializeField] private float delayToSpawn = 0.5f;
    [SerializeField] private int numberOfCoins = 10;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float burstDuration = 0.2f;

    private void Start()
    {
        Invoke(nameof(ExplodeCoins), delayToSpawn);    
    }

    private void ExplodeCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * explosionRadius;
            CoinDrop newCoin = Instantiate(coinDrop, randomPosition, Quaternion.identity);
            Vector2 randomDirection = (randomPosition - (Vector2)transform.position).normalized;
            Rigidbody2D rb = newCoin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(randomDirection * explosionForce, ForceMode2D.Impulse);
                newCoin.StopForce(burstDuration);
            }
        }
        Destroy(gameObject);
    }
}
