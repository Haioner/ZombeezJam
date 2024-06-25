using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float YOffset;
    [SerializeField] private float explosiveDamage;
    [SerializeField] private float explosiveRange;
    private HealthController healthController;

    private void Start()
    {
        healthController = GetComponent<HealthController>();
    }

    public void Explode()
    {
        Vector3 offsetPos = transform.position;
        offsetPos.y += YOffset;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(offsetPos, explosiveRange);

        foreach (Collider2D hitCollider in hitColliders)
        {
            HealthController healthController = hitCollider.GetComponent<HealthController>();
            if (healthController != null && this.healthController != healthController)
            {
                healthController.TakeDamage(explosiveDamage);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 offsetPos = transform.position;
        offsetPos.y += YOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(offsetPos, explosiveRange);
    }
}
