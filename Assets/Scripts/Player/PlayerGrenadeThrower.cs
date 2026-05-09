using UnityEngine;

// Этот скрипт отвечает за бросок гранаты.
// Теперь гранаты ограничены по количеству.
// Игрок не может бросать гранаты бесконечно.
public class PlayerGrenadeThrower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    // Точка, откуда появляется граната.

    [SerializeField] private Transform playerVisual;
    // Визуальная часть игрока.

    [SerializeField] private GameObject grenadePrefab;
    // Prefab гранаты.

    [SerializeField] private PlayerInputReader inputReader;
    // Источник ввода.

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 10f;
    // Сила броска вперёд.

    [SerializeField] private float upwardForce = 2f;
    // Подброс вверх.

    [SerializeField] private float throwCooldown = 3f;
    // Кулдаун между бросками.

    [Header("Ammo Settings")]
    [SerializeField] private int startGrenades = 2;
    // Сколько гранат у игрока в начале игры.

    [SerializeField] private int maxGrenades = 3;
    // Максимум гранат, который игрок может носить.

    private int currentGrenades;
    // Текущее количество гранат.

    private float currentCooldown;
    // Остаток кулдауна.

    private void Awake()
    {
        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        currentGrenades = Mathf.Clamp(startGrenades, 0, maxGrenades);
    }

    private void Update()
    {
        UpdateCooldown();
        HandleGrenadeInput();
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

    private void HandleGrenadeInput()
    {
        if (inputReader == null)
        {
            return;
        }

        if (!inputReader.GrenadeReleased)
        {
            return;
        }

        if (currentCooldown > 0f)
        {
            return;
        }

        // Если гранат нет, бросок не выполняем.
        if (currentGrenades <= 0)
        {
            return;
        }

        Vector3 throwDirection = GetThrowDirection();

        if (throwDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        ThrowGrenade(throwDirection);
    }

    private void ThrowGrenade(Vector3 throwDirection)
    {
        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint is not assigned.");
            return;
        }

        if (grenadePrefab == null)
        {
            Debug.LogWarning("Grenade Prefab is not assigned.");
            return;
        }

        GameObject spawnedGrenade = Instantiate(
            grenadePrefab,
            firePoint.position,
            Quaternion.LookRotation(throwDirection)
        );

        Rigidbody grenadeRb = spawnedGrenade.GetComponent<Rigidbody>();

        if (grenadeRb != null)
        {
            Vector3 finalVelocity = throwDirection * throwForce + Vector3.up * upwardForce;
            grenadeRb.velocity = finalVelocity;
        }

        // Тратим одну гранату после успешного броска.
        currentGrenades--;

        currentCooldown = throwCooldown;
    }

    private Vector3 GetThrowDirection()
    {
        if (inputReader != null && inputReader.HasGrenadeAimDirection)
        {
            Vector3 grenadeDirection = inputReader.GrenadeAimDirection;
            grenadeDirection.y = 0f;

            if (grenadeDirection.sqrMagnitude > 0.001f)
            {
                return grenadeDirection.normalized;
            }
        }

        if (inputReader != null)
        {
            Vector3 aimDirection = inputReader.AimDirection;
            aimDirection.y = 0f;

            if (aimDirection.sqrMagnitude > 0.001f)
            {
                return aimDirection.normalized;
            }
        }

        if (playerVisual != null)
        {
            Vector3 visualForward = playerVisual.forward;
            visualForward.y = 0f;

            if (visualForward.sqrMagnitude > 0.001f)
            {
                return visualForward.normalized;
            }
        }

        Vector3 rootForward = transform.forward;
        rootForward.y = 0f;

        if (rootForward.sqrMagnitude > 0.001f)
        {
            return rootForward.normalized;
        }

        return Vector3.zero;
    }

    public bool IsOnCooldown()
    {
        return currentCooldown > 0f;
    }

    public float GetCurrentCooldown()
    {
        return currentCooldown;
    }

    public int GetCurrentGrenades()
    {
        return currentGrenades;
    }

    public int GetMaxGrenades()
    {
        return maxGrenades;
    }

    public void AddGrenades(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentGrenades += amount;

        if (currentGrenades > maxGrenades)
        {
            currentGrenades = maxGrenades;
        }
    }
}