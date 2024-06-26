using System.Collections;
using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    [SerializeField] private int CoinValue;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private ParticleSystem coinParticle;
    private bool hasCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasCollected)
        {
            hasCollected = true;
            GameManager.instance.AddCoin(CoinValue);
            SoundManager.PlayAudioClip(coinClip);
            Instantiate(coinParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void StopForce(float time)
    {
        StartCoroutine(StopForceCoroutine(time));
    }

    private IEnumerator StopForceCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
    }
}
