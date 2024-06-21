using UnityEngine;

[System.Serializable]
public class WeaponStatistics
{
    public int WeaponTier;
    public float Damage;
    public float Spread;
    public float Rate;
    public float AmmoAmount;
    public float ReloadTime;
}

public class WeaponController : MonoBehaviour
{
    [Header("Weapon")]
    public WeaponSO weaponSO;
    private SpriteRenderer spriteRenderer;
    public event System.EventHandler OnBulletChanged;
    public event System.EventHandler OnReload;
    public event System.EventHandler OnShoot;

    [Header("Statistics")]
    public WeaponStatistics statistics;

    [Header("Bullets")]
    public int currentBullets = 0;
    public int inventoryBullets;

    //Spin
    private float nextFireTime;
    private bool inCooldown;

    //Reload
    private float reloadTime;
    private bool isReloading;

    //Cache
    private RotateTowardsMouse rotateTowards;
    private AudioSource audioSource;
    private GameObject playerObject;
    private OptionsManager optionsManager;

    private void Start()
    {
        reloadTime = statistics.ReloadTime;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rotateTowards = GetComponent<RotateTowardsMouse>();
        audioSource = GetComponent<AudioSource>();
        playerObject = GetComponentInParent<PlayerManager>().gameObject;
        optionsManager = FindObjectOfType<OptionsManager>();

        SetWeaponSprite();
        OnBulletChanged?.Invoke(this, System.EventArgs.Empty);
    }

    private void Update()
    {
        Shoot();
        CalculateReload();

        if (Input.GetKeyDown(KeyCode.R))
            ReloadGun();
    }

    private void FixedUpdate()
    {
        SpinByRate();
    }

    public void AddInventoryAmmo(int amount)
    {
        inventoryBullets += amount;
        OnBulletChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public void SwitchWeapon(WeaponSO weapon, int currentBullets, int inventoryBullets, WeaponStatistics statistics)
    {
        weaponSO = weapon;
        this.currentBullets = currentBullets;
        this.inventoryBullets = inventoryBullets;
        this.statistics = statistics;
        SetWeaponSprite();

        isReloading = false;
        reloadTime = statistics.ReloadTime;

        OnBulletChanged?.Invoke(this, System.EventArgs.Empty);
    }

    private void SetWeaponSprite()
    {
        spriteRenderer.sprite = weaponSO.WeaponSprite;

        if (weaponSO.itemHandType == ItemHandType.Single)
            transform.localPosition = new Vector3(0.34f, 0.214f, 0f);
        else
            transform.localPosition = new Vector3(0.582f, -0.02f, 0f);
    }

    private void Shoot()
    {
        if (currentBullets <= 0)
        {
            if (Input.GetMouseButton(0))
                ReloadGun();

            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && rotateTowards.isActiveAndEnabled && !isReloading)
        {
            if (optionsManager.GetOptionsState()) return;

            CinemachineShake.instance.ShakeCamera(1f, 0.1f);
            PlayClipAudio(weaponSO.ShootClip);

            float angle = CalculateShootAngle();
            SpawnShootFire(angle);

            //Spawn Bullets per shoot
            if (weaponSO.BulletsPerShoot <= 1)
                SpawnBullet(angle);
            else
            {
                for (int i = 0; i < weaponSO.BulletsPerShoot; i++)
                {
                    float anglePerBullet = CalculateShootAngle();
                    SpawnBullet(anglePerBullet);
                }
            }

            CartRidgeShoot();

            //Restart Rate
            nextFireTime = Time.time + 1f / statistics.Rate;

            OnBulletChanged?.Invoke(this, System.EventArgs.Empty);
            OnShoot?.Invoke(this, System.EventArgs.Empty);
        }
    }

    private void CartRidgeShoot()
    {
        currentBullets--;
    }

    private void ReloadGun()
    {
        if (inventoryBullets > 0 && !isReloading && currentBullets < (int)statistics.AmmoAmount)
        {
            isReloading = true;
            PlayClipAudio(weaponSO.StartReloadClip);
            Instantiate(weaponSO.ReloadParticle, transform.parent.position, Quaternion.identity);
            OnReload?.Invoke(this, System.EventArgs.Empty);
        }
    }

    private void CalculateReload()
    {
        if (reloadTime > 0 && isReloading)
            reloadTime -= Time.deltaTime;
        else if (reloadTime <= 0)
        {
            float bulletsToReload = statistics.AmmoAmount - currentBullets;
            if (inventoryBullets > 0 && bulletsToReload > 0)
            {
                PlayClipAudio(weaponSO.EndReloadClip);
                currentBullets += (int)Mathf.Min(bulletsToReload, inventoryBullets);
                inventoryBullets -= (int)Mathf.Min(bulletsToReload, inventoryBullets);

                isReloading = false;
                reloadTime = statistics.ReloadTime;
                OnBulletChanged?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }

    private void SpinByRate()
    {
        if (Time.time < nextFireTime)
            inCooldown = true;
        else
            inCooldown = false;

        if (inCooldown)
        {
            rotateTowards.enabled = false;
            float degreesPerSecond = 360f * statistics.Rate;
            transform.parent.Rotate(Vector3.forward, degreesPerSecond * Time.deltaTime);
        }
        else
        {
            rotateTowards.enabled = true;
        }
    }

    private void SpawnShootFire(float angle)
    {
        ParticleSystem fire = Instantiate(weaponSO.ShootFireParticle, transform.position, Quaternion.identity);
        var main = fire.main;
        main.startRotation = -angle * Mathf.Deg2Rad;

        Vector3 fireDirection = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        fire.transform.rotation = Quaternion.LookRotation(Vector3.forward, fireDirection);
    }

    private void SpawnBullet(float angle)
    {
        Projectile newBullet = Instantiate(weaponSO.BulletPrefab, transform.position, Quaternion.identity);
        newBullet.InitProjectile(angle, statistics.Damage, playerObject);
    }

    private float CalculateShootAngle()
    {
        //Mouse Position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector3 direction = (mousePosition - transform.position).normalized;

        //Spread
        float randomAngle = Random.Range(-statistics.Spread, statistics.Spread);

        //Calculate angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += randomAngle;
        return angle;
    }

    private void PlayClipAudio(AudioClip audioClip)
    {
        float randPitch = Random.Range(weaponSO.RandShootPitch.x, weaponSO.RandShootPitch.y);
        audioSource.pitch = randPitch;
        audioSource.PlayOneShot(audioClip);
    }
}
