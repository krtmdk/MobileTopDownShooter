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
    // Точка, из которой создаются пули.

    [SerializeField] private GameObject projectilePrefab;
    // Prefab пули.

    [SerializeField] private PlayerInputReader inputReader;
    // Ссылка на источник ввода.

    [Header("Current Weapon")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.Rifle;
    // Текущее выбранное оружие.

    [Header("Rifle Settings")]
    [SerializeField] private float rifleFireCooldown = 0.12f;
    // Задержка между выстрелами автомата.

    [SerializeField] private int rifleDamage = 1;
    // Урон автомата.

    [SerializeField] private int rifleMaxAmmo = 20;
    // Размер магазина автомата.

    [SerializeField] private float rifleReloadTime = 1.5f;
    // Время перезарядки автомата.

    [SerializeField] private float rifleStandingSpreadAngle = 3f;
    // Разброс автомата, если игрок стоит.

    [SerializeField] private float rifleMovingSpreadAngle = 15f;
    // Разброс автомата, если игрок движется.

    [Header("Shotgun Settings")]
    [SerializeField] private float shotgunFireCooldown = 0.8f;
    // Задержка между выстрелами дробовика.

    [SerializeField] private int shotgunDamage = 1;
    // Урон одной дробины.

    [SerializeField] private int shotgunPelletCount = 6;
    // Количество дробин за один выстрел.

    [SerializeField] private int shotgunMaxAmmo = 6;
    // Размер магазина дробовика.

    [SerializeField] private float shotgunReloadTime = 2f;
    // Время перезарядки дробовика.

    [SerializeField] private float shotgunStandingSpreadAngle = 10f;
    // Разброс дробовика, если игрок стоит.

    [SerializeField] private float shotgunMovingSpreadAngle = 20f;
    // Разброс дробовика, если игрок движется.

    [Header("Movement Accuracy Settings")]
    [SerializeField] private float movingSpeedThreshold = 0.1f;
    // Порог скорости, после которого считаем, что игрок движется.

    private int currentAmmo;
    // Текущее количество патронов в магазине.

    private bool isReloading;
    // Идёт ли сейчас перезарядка.

    private float currentCooldown;
    // Остаток времени до следующего выстрела.

    private Collider playerCollider;
    // Коллайдер игрока.

    private Rigidbody playerRigidbody;
    // Rigidbody игрока.

    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        playerRigidbody = GetComponent<Rigidbody>();

        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        // На старте заполняем магазин текущего оружия.
        currentAmmo = GetCurrentWeaponMaxAmmo();
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

        // Во время перезарядки стрелять нельзя.
        if (isReloading)
        {
            return;
        }

        // Если кнопка стрельбы не активна, не стреляем.
        if (!inputReader.IsShooting)
        {
            return;
        }

        // Если кулдаун ещё не закончился, не стреляем.
        if (currentCooldown > 0f)
        {
            return;
        }

        // Если патронов нет, пытаемся начать перезарядку.
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
        // Добавляем разброс только по горизонтали,
        // чтобы top-down стрельба читалась стабильнее.
        Quaternion spreadRotation = Quaternion.Euler(
            0f,
            Random.Range(-spreadAngle, spreadAngle),
            0f
        );

        Quaternion finalRotation = firePoint.rotation * spreadRotation;

        // Немного смещаем точку появления, чтобы снаряд не рождался
        // слишком глубоко внутри игрока.
        Vector3 spawnPosition = firePoint.position + finalRotation * Vector3.forward * 0.2f;

        GameObject spawnedProjectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            finalRotation
        );

        Collider projectileCollider = spawnedProjectile.GetComponent<Collider>();

        // Игнорируем столкновение своей пули с игроком.
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

        // Если шла перезарядка, прерываем её.
        StopAllCoroutines();
        isReloading = false;

        currentWeapon = newWeapon;
        currentAmmo = GetCurrentWeaponMaxAmmo();
        currentCooldown = 0f;
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

    // ===== Методы для UI =====

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