using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightClipping : MonoBehaviour
{
    [SerializeField] private Light2D light2D;

    private int shadowCastersInContact = 0;

    private void Start()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out ShadowCaster2D shadow))
            {
                shadowCastersInContact++;
            }
        }

        light2D.enabled = shadowCastersInContact == 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ShadowCaster2D shadow))
        {
            shadowCastersInContact++;
            light2D.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ShadowCaster2D shadow))
        {
            shadowCastersInContact--;
            if (shadowCastersInContact <= 0)
            {
                shadowCastersInContact = 0;
                light2D.enabled = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ShadowCaster2D shadow))
        {
            light2D.enabled = false;
        }
    }
}
