using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    public float MaxSpeed = 5f;

    [Header("Health")]
    public float MaxHealth = 100f;

    [Header("Weapon")]
    public WeaponController weaponController;

    [Header("Animation")]
    public Animator anim;

    private void Update()
    {
        UpdateWeaponAnim();
    }

    private void UpdateWeaponAnim()
    {
        switch (weaponController.weaponSO.itemHandType)
        {
            case ItemHandType.Single: anim.SetFloat("WeaponCount", 0); break;
            case ItemHandType.Double: anim.SetFloat("WeaponCount", 1); break;
        }
    }
}
