using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private ParticleSystem hitParticle;

    private float projectileDamage;
    private Vector2 moveDirection;
    private GameObject owner;
    private Rigidbody2D rb;
    private int damageCount = 2;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f);
    }

    public void InitProjectile(float angle, float damage, GameObject owner)
    {
        moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        projectileDamage = damage;
        this.owner = owner;

        GetComponent<CircleCollider2D>().excludeLayers = LayerMask.GetMask(LayerMask.LayerToName(owner.layer));
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        rb.velocity = moveDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != owner.gameObject)
        {
            HealthController healthController = collision.GetComponentInParent<HealthController>();
            if (collision.CompareTag("Head"))
            {
                
                // Insta-kill logic
                if (healthController != null && damageCount > 0)
                {
                    healthController.HeadShotDamage(healthController.GetCurrentHealth());
                    damageCount-= 10;
                }
            }
            else if (healthController != null && damageCount > 0)
            {
                healthController.TakeDamage(projectileDamage);
                damageCount--;
            }

            Instantiate(hitParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
