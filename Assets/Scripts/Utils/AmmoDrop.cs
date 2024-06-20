using UnityEngine;
using TMPro;

public class AmmoDrop : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxRandomAmmo;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private FloatNumber floatNumberPrefab;
    [SerializeField] private AudioClip ammoClip;
    [SerializeField] private float audioVolume = 1f;
    private int randAmmo;

    private void Start()
    {
        randAmmo = (int)Random.Range(minMaxRandomAmmo.x, minMaxRandomAmmo.y);
        ammoText.text = randAmmo.ToString("F0");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponentInChildren<WeaponController>().AddInventoryAmmo(randAmmo);

            SoundManager.PlayAudioClipVolume(ammoClip, audioVolume);

            FloatNumber currentFloatNumber = Instantiate(floatNumberPrefab, transform.position, Quaternion.identity);
            string ammoText = "<sprite=4> " + randAmmo.ToString();
            currentFloatNumber.InitFloatNumber(ammoText, Color.white);

            Destroy(gameObject);
        }
    }
}
