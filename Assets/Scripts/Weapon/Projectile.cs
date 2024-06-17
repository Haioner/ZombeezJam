using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float projectileDamage;
    private Vector2 moveDirection;
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

    public void InitProjectile(float angle, float damage)
    {
        moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        projectileDamage = damage;
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        rb.velocity = moveDirection * speed;
    }
}
