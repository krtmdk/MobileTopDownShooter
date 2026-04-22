using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerShooter : MonoBehaviour
{
    private enum WeaponType
    {
        Rifle,
        Shotgun
    }

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Animator playerAnimator;

    [Header("Weapon Visuals")]
    [SerializeField] private GameObject rifleVisual;
    [SerializeField] private GameObject shotgunVisual;

    [Header("Current Weapon")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.Rifle;

    [Header("Rifle Settings")]
    [SerializeField] private float rifleFireCooldown = 0.12f;
    [SerializeField] private int rifleDamage = 1;
    [SerializeField] private int rifleMaxAmmo = 20;
    [SerializeField] private float rifleReloadTime = 1.5f;
    [SerializeField] private float rifleStandingSpreadAngle = 3f;
    [SerializeField] private float rifleMovingSpreadAngle = 15f;

    [Header("Shotgun Settings")]
    [SerializeField] private float shotgunFireCooldown = 0.8f;
    [SerializeField] private int shotgunDamage = 1;
    [SerializeField] private int shotgunPelletCount = 6;
    [SerializeField] private int shotgunMaxAmmo = 6;
    [SerializeField] private float shotgunReloadTime = 2f;
    [SerializeField] private float shotgunStandingSpreadAngle = 10f;
    [SerializeField] private float shotgunMovingSpreadAngle = 20f;

    [Header("Movement Accuracy Settings")]
    [SerializeField] private float movingSpeedThreshold = 0.1f;

    private int currentAmmo;
    private bool isReloading;
    private float currentCooldown;
    private Collider playerCollider;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        playerRigidbody = GetComponent<Rigidbody>();

        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        currentAmmo = GetCurrentWeaponMaxAmmo();
        UpdateWeaponVisuals();
    }

    private void Update()
    {
        UpdateCooldown();
        HandleWeaponSwitch();
        HandleReload();
        HandleShoot();
    }

    private void UpdateCooldown()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown < 0f)
            {
                currentCooldown = 0f;
            }
        }
    }

    private void HandleWeaponSwitch()
    {
        if (inputReader == null)
        {
            return;
        }

        if (inputReader.SwitchToRifle)
        {
            SwitchWeapon(WeaponType.Rifle);
        }

        if (inputReader.SwitchToShotgun)
        {
            SwitchWeapon(WeaponType.Shotgun);
        }
    }

    private void HandleReload()
    {
        if (inputReader == null)
        {
            return;
        }

        if (inputReader.ReloadPressed)
        {
            TryReload();
        }
    }

    private void HandleShoot()
    {
        if (inputReader == null)
        {
            return;
        }

        if (isReloading)
        {
            return;
        }

        if (!inputReader.IsShooting)
        {
            return;
        }

        if (currentCooldown > 0f)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            TryReload();
            return;
        }

        Shoot();
    }

    private void Shoot()
    {
        if (firePoint == null || projectilePrefab == null)
        {
            return;
        }

        bool isMoving = IsPlayerMoving();

        switch (currentWeapon)
        {
            case WeaponType.Rifle:
                ShootRifle(isMoving);
                break;

            case WeaponType.Shotgun:
                ShootShotgun(isMoving);
                break;
        }

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Shoot");
            playerAnimator.SetTrigger("Shoot");
        }
    }

    private void ShootRifle(bool isMoving)
    {
        float spreadAngle = isMoving ? rifleMovingSpreadAngle : rifleStandingSpreadAngle;
        SpawnProjectileWithSpread(spreadAngle, rifleDamage);
        currentAmmo--;
        currentCooldown = rifleFireCooldown;
    }

    private void ShootShotgun(bool isMoving)
    {
        float spreadAngle = isMoving ? shotgunMovingSpreadAngle : shotgunStandingSpreadAngle;

        for (int i = 0; i < shotgunPelletCount; i++)
        {
            SpawnProjectileWithSpread(spreadAngle, shotgunDamage);
        }

        currentAmmo--;
        currentCooldown = shotgunFireCooldown;
    }

    private void SpawnProjectileWithSpread(float spreadAngle, int damage)
    {
        Quaternion spreadRotation = Quaternion.Euler(
            0f,
            Random.Range(-spreadAngle, spreadAngle),
            0f
        );

        Quaternion finalRotation = firePoint.rotation * spreadRotation;
        Vector3 spawnPosition = firePoint.position + finalRotation * Vector3.forward * 0.2f;

        GameObject spawnedProjectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            finalRotation
        );

        Collider projectileCollider = spawnedProjectile.GetComponent<Collider>();

        if (projectileCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, playerCollider);
        }

        Projectile projectile = spawnedProjectile.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetDamage(damage);
        }
    }

    private void TryReload()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo >= GetCurrentWeaponMaxAmmo())
        {
            return;
        }

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Reload");
            playerAnimator.SetTrigger("Reload");
        }

        yield return new WaitForSeconds(GetCurrentWeaponReloadTime());

        currentAmmo = GetCurrentWeaponMaxAmmo();
        isReloading = false;
    }

    private void SwitchWeapon(WeaponType newWeapon)
    {
        if (currentWeapon == newWeapon)
        {
            return;
        }

        StopAllCoroutines();
        isReloading = false;

        currentWeapon = newWeapon;
        currentAmmo = GetCurrentWeaponMaxAmmo();
        currentCooldown = 0f;

        UpdateWeaponVisuals();
    }

    private void UpdateWeaponVisuals()
    {
        if (rifleVisual != null)
        {
            rifleVisual.SetActive(currentWeapon == WeaponType.Rifle);
        }

        if (shotgunVisual != null)
        {
            shotgunVisual.SetActive(currentWeapon == WeaponType.Shotgun);
        }
    }

    private bool IsPlayerMoving()
    {
        if (playerRigidbody == null)
        {
            return false;
        }

        Vector3 horizontalVelocity = playerRigidbody.velocity;
        horizontalVelocity.y = 0f;

        return horizontalVelocity.magnitude > movingSpeedThreshold;
    }

    private int GetCurrentWeaponMaxAmmo()
    {
        switch (currentWeapon)
        {
            case WeaponType.Rifle:
                return rifleMaxAmmo;
            case WeaponType.Shotgun:
                return shotgunMaxAmmo;
        }

        return rifleMaxAmmo;
    }

    private float GetCurrentWeaponReloadTime()
    {
        switch (currentWeapon)
        {
            case WeaponType.Rifle:
                return rifleReloadTime;
            case WeaponType.Shotgun:
                return shotgunReloadTime;
        }

        return rifleReloadTime;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetCurrentWeaponMaxAmmoForUI()
    {
        return GetCurrentWeaponMaxAmmo();
    }

    public string GetCurrentWeaponName()
    {
        return currentWeapon.ToString();
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}