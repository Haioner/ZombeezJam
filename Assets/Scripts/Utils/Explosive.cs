using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float YOffset;
    [SerializeField] private float explosiveDamage;
    [SerializeField] private float explosiveRange;
    private HealthController healthController;
    [HideInInspector] public bool hasExploded;

    private void Start()
    {
        healthController = GetComponent<HealthController>();
    }

    public void Explode()
    {
        hasExploded = true;
        Vector3 offsetPos = transform.position;
        offsetPos.y += YOffset;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(offsetPos, explosiveRange);

        foreach (Collider2D hitCollider in hitColliders)
        {
            HealthController healthController = hitCollider.GetComponent<HealthController>();
            Explosive explosiveObj = hitCollider.GetComponent<Explosive>();

            if(healthController != null && gameObject != hitCollider.gameObject)
            {
                if (explosiveObj != null)
                {
                   if(!explosiveObj.hasExploded)
                        healthController.TakeDamage(explosiveDamage + (GameManager.instance.CurrentRoom * 15f));
                }
                else
                {
                    healthController.TakeDamage(explosiveDamage + (GameManager.instance.CurrentRoom * 15f));
                }
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
