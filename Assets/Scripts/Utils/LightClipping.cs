using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class LightClipping : MonoBehaviour
{
    [SerializeField] private Light2D light2D;
    private CircleCollider2D circleCollider;
    private float initialRadius;
    private int shadowCastersInContact = 0;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        initialRadius = circleCollider.radius;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out TilemapCollider2D shadow))
            {
                shadowCastersInContact++;
            }
        }

        light2D.enabled = shadowCastersInContact == 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out TilemapCollider2D shadow))
        {
            shadowCastersInContact++;
            light2D.enabled = false;
            circleCollider.radius = initialRadius * 3.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out TilemapCollider2D shadow))
        {
            shadowCastersInContact--;
            if (shadowCastersInContact <= 0)
            {
                shadowCastersInContact = 0;
                light2D.enabled = true;
                circleCollider.radius = initialRadius;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out TilemapCollider2D shadow))
        {
            light2D.enabled = false;
            circleCollider.radius = initialRadius * 3.5f;
        }
    }
}
