using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private ParticleSystem hitParticle;

    private float projectileDamage;
    private Vector2 moveDirection;
    private GameObject owner;
    private Rigidbody2D rb;

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
            if (collision.TryGetComponent(out HealthController healthController))
                healthController.TakeDamage(projectileDamage);

            Instantiate(hitParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
