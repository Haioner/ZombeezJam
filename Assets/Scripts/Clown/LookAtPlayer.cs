using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform gfxHolder;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (gfxHolder.localScale.x <= -1)
                transform.localScale = new Vector3(1, 1, transform.localScale.z);
            else
                transform.localScale = new Vector3(-1, -1, transform.localScale.z);
        }
    }
}
