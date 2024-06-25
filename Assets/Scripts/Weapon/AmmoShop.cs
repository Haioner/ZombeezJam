using UnityEngine;
using TMPro;

public class AmmoShop : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject priceCanvas;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private FloatNumber floatNumberPrefab;
    [SerializeField] private AudioClip shopClip;
    public bool IsNearest { get; set; }

    private int currentPrice;
    private int boughtCount;

    private void Start()
    {
        currentPrice = 5;
        UpdatePrice();
    }

    private void Update()
    {
        StatsCanvas();
    }

    private void StatsCanvas()
    {
        priceCanvas.SetActive(IsNearest);
    }

    private void UpdatePrice()
    {
        priceText.text = "<sprite=0>" + currentPrice.ToString();
    }

    private void AddPrice()
    {
        boughtCount++;
        currentPrice = GameManager.instance.CurrentRoom - 5 + boughtCount;
    }

    public void Interact()
    {
        if(GameManager.instance.Coins >= currentPrice)
        {
            GameManager.instance.SubtractCoin(currentPrice);
            AddPrice();
            UpdatePrice();
            FindObjectOfType<WeaponController>().AddInventoryAmmo(25);
            SoundManager.PlayAudioClip(shopClip);
        }
        else
        {
            FloatNumber floatNumber = Instantiate(floatNumberPrefab, transform.position, Quaternion.identity);
            floatNumber.InitFloatNumber("Not enough coins", Color.red);
        }

    }
}
