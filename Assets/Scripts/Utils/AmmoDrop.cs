using UnityEngine;
using TMPro;

public class AmmoDrop : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxRandomAmmo;
    [SerializeField] private TextMeshProUGUI ammoText;
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
            Destroy(gameObject);
        }
    }
}
