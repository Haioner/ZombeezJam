using UnityEngine;

public class RoomDestroyer : MonoBehaviour
{
    [SerializeField] private GameObject preventBackCollider;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("CheckDistance", 2, 2);
    }

    private void CheckDistance()
    {
        if (player.position.x < transform.position.x) return;

        if(PlayerIsBetween(100,150))
        {
            if (preventBackCollider != null)
                preventBackCollider.SetActive(true);
        }
        else if(PlayerIsBetween(150, Mathf.Infinity))
        {
            Destroy(gameObject);
        }
    }

    private bool PlayerIsBetween(float distanceMin, float distanceMax)
    {
        return PlayerDistance() > distanceMin && PlayerDistance() < distanceMax;
    }

    private float PlayerDistance()
    {
        return Vector2.Distance(transform.position, player.position);
    }
}
