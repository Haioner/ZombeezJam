using UnityEngine;

[System.Serializable]
public enum ItemHandType
{
    Single, Double
}

[CreateAssetMenu(menuName ="Weapon")]
public class WeaponSO : ScriptableObject
{
    [Header("Weapon")]
    public ItemHandType itemHandType;
    public Sprite WeaponSprite;
    public Sprite WeaponOutlineSprite;
    public float Spread = 5f;
    public float Rate = 1f;
    public int CartridgeAmount = 13;
    public float ReloadCooldown = 1f;
    public int BulletsPerShoot = 1;

    [Header("Bullet")]
    public float BulletDamage = 10f;
    public Projectile BulletPrefab;

    [Header("Audios")]
    public Vector2 RandShootPitch = new Vector2(1f, 1f);
    public AudioClip ShootClip;
    public AudioClip StartReloadClip;
    public AudioClip EndReloadClip;

    [Header("Particles")]
    public ParticleSystem ShootFireParticle;
    public ParticleSystem ReloadParticle;
}
