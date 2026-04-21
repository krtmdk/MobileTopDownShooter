using UnityEngine;

// Этот скрипт отвечает за бросок гранаты.
// На ПК граната летит по направлению мыши.
// На мобильном граната летит по направлению drag-наведения.
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

    private float currentCooldown;
    // Остаток кулдауна.

    private void Awake()
    {
        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }
    }

    private void Update()
    {
        UpdateCooldown();
        HandleGrenadeInput();
    }

    // Этот метод уменьшает кулдаун каждый кадр.
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

    // Этот метод проверяет, нужно ли сейчас бросать гранату.
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

        Vector3 throwDirection = GetThrowDirection();

        // Если не удалось получить валидное направление, ничего не делаем.
        if (throwDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        ThrowGrenade(throwDirection);
    }

    // Этот метод создаёт гранату и задаёт ей скорость.
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

        currentCooldown = throwCooldown;
    }

    // Этот метод выбирает направление броска.
    private Vector3 GetThrowDirection()
    {
        // 1. Сначала пытаемся взять специальное направление гранаты.
        if (inputReader != null && inputReader.HasGrenadeAimDirection)
        {
            Vector3 grenadeDirection = inputReader.GrenadeAimDirection;
            grenadeDirection.y = 0f;

            if (grenadeDirection.sqrMagnitude > 0.001f)
            {
                return grenadeDirection.normalized;
            }
        }

        // 2. Если его нет, используем обычное направление прицеливания.
        if (inputReader != null)
        {
            Vector3 aimDirection = inputReader.AimDirection;
            aimDirection.y = 0f;

            if (aimDirection.sqrMagnitude > 0.001f)
            {
                return aimDirection.normalized;
            }
        }

        // 3. Затем пробуем направление визуала игрока.
        if (playerVisual != null)
        {
            Vector3 visualForward = playerVisual.forward;
            visualForward.y = 0f;

            if (visualForward.sqrMagnitude > 0.001f)
            {
                return visualForward.normalized;
            }
        }

        // 4. Самый запасной вариант.
        Vector3 rootForward = transform.forward;
        rootForward.y = 0f;

        if (rootForward.sqrMagnitude > 0.001f)
        {
            return rootForward.normalized;
        }

        return Vector3.zero;
    }

    // Этот метод нужен UI, чтобы проверить кулдаун.
    public bool IsOnCooldown()
    {
        return currentCooldown > 0f;
    }

    // Этот метод нужен UI, чтобы показать оставшееся время.
    public float GetCurrentCooldown()
    {
        return currentCooldown;
    }
}