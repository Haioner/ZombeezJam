using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    [SerializeField] private int CoinValue;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private ParticleSystem coinParticle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.AddCoin(CoinValue);
            SoundManager.PlayAudioClip(coinClip);
            Instantiate(coinParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
