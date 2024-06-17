using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponDrop : MonoBehaviour, IInteractable
{
    [Header("Weapon")]
    [SerializeField] private WeaponSO weaponSO;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool IsNearest { get; set; }

    [Header("Bullets")]
    [SerializeField] private int currentBullets = 0;
    [SerializeField] private int inventoryBullets;

    [Header("Canvas")]
    [SerializeField] private GameObject statsCanvas;
    [SerializeField] private TextMeshProUGUI statsText;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        UpdateSprite();
        RandBullets();
    }

    private void Update()
    {
        Outline();
        StatsCanvas();
    }

    private void RandBullets()
    {
        int randCurrent = Random.Range(0, weaponSO.CartridgeAmount);
        currentBullets = randCurrent;

        int randInventory = Random.Range(0, 100);
        inventoryBullets = randInventory;
    }

    private void Outline()
    {
        if (IsNearest)
            spriteRenderer.sprite = weaponSO.WeaponOutlineSprite;
        else
            spriteRenderer.sprite = weaponSO.WeaponSprite;
    }

    private void StatsCanvas()
    {
        statsCanvas.SetActive(IsNearest);
        statsText.text = "<sprite=0> " + (inventoryBullets + currentBullets).ToString("F0");
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = weaponSO.WeaponSprite;

        if (weaponSO.itemHandType == ItemHandType.Single)
            spriteRenderer.transform.localPosition = new Vector3(0.275f, 0.133f, 0);
        else
            spriteRenderer.transform.localPosition = new Vector3(0.561f, 0.033f, 0);
    }

    public void Interact()
    {
        audioSource.PlayOneShot(weaponSO.EndReloadClip);

        WeaponController weaponController = FindObjectOfType<WeaponController>();

        //Save drop info
        WeaponSO lastWeapon = weaponSO;
        int lastCurrentBullets = currentBullets;
        int lastInventoryBullets = inventoryBullets;

        //Change this drop
        weaponSO = weaponController.weaponSO;
        currentBullets = weaponController.currentBullets;
        inventoryBullets = weaponController.inventoryBullets;
        UpdateSprite();

        //Change player weapon
        weaponController.SwitchWeapon(lastWeapon, lastCurrentBullets, lastInventoryBullets);
    }
}
