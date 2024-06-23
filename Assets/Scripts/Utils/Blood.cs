using UnityEngine;

public class Blood : MonoBehaviour
{
    [SerializeField] private Sprite[] bloodsSprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 minMaxStartScale;

    private void Start()
    {
        spriteRenderer.sprite = bloodsSprites[Random.Range(0, bloodsSprites.Length)];

        float randomScale = Random.Range(minMaxStartScale.x, minMaxStartScale.y);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);

        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
