using UnityEngine;
using TMPro;

public class Collectable : MonoBehaviour
{
    [Header("Collectable")]
    [SerializeField] private Vector2 minMaxRandomValue;
    [SerializeField] private TextMeshProUGUI worldText;
    [SerializeField] private FloatNumber floatNumberPrefab;
    [SerializeField] private string floatMessage;

    [Header("Audio")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private float audioVolume = 1f;

    protected int randValue;
    protected Collider2D objectCollider;

    private void Start()
    {
        randValue = (int)Random.Range(minMaxRandomValue.x, minMaxRandomValue.y);

        if (worldText != null)
            worldText.text = randValue.ToString("F0");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            objectCollider = collision;

            if (CanCollect())
            {
                OnCollect(collision);
                SoundManager.PlayAudioClipVolume(collectClip, audioVolume);
                Destroy(gameObject);
            }
        }
    }

    public virtual void OnCollect(Collider2D collision)
    {
        FloatNumber currentFloatNumber = Instantiate(floatNumberPrefab, transform.position, Quaternion.identity);
        currentFloatNumber.InitFloatNumber(floatMessage + randValue.ToString("F0"), Color.white);
    }

    public virtual bool CanCollect()
    {
        bool state = false;
        return state;
    }
}