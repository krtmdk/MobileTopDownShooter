using UnityEngine;

// Этот скрипт отвечает за установку клеймора.
// На ПК клеймор ставится по направлению мыши.
// На мобильном клеймор ставится по направлению drag-наведения.
public class PlayerClaymorePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerVisual;
    // Визуальная часть игрока.

    [SerializeField] private GameObject claymorePrefab;
    // Prefab клеймора.

    [SerializeField] private PlayerInputReader inputReader;
    // Источник ввода.

    [Header("Placement Settings")]
    [SerializeField] private float placeDistance = 1.2f;
    // Дистанция установки клеймора перед игроком.

    [SerializeField] private float placeCooldown = 3f;
    // Кулдаун между установками.

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
        HandleClaymoreInput();
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

    // Этот метод проверяет, нужно ли сейчас ставить клеймор.
    private void HandleClaymoreInput()
    {
        if (inputReader == null)
        {
            return;
        }

        if (!inputReader.ClaymoreReleased)
        {
            return;
        }

        if (currentCooldown > 0f)
        {
            return;
        }

        Vector3 placementDirection = GetPlacementDirection();

        if (placementDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        PlaceClaymore(placementDirection);
    }

    // Этот метод создаёт клеймор в нужной позиции и с нужным поворотом.
    private void PlaceClaymore(Vector3 placementDirection)
    {
        if (claymorePrefab == null)
        {
            Debug.LogWarning("Claymore Prefab is not assigned.");
            return;
        }

        Vector3 placePosition = transform.position + placementDirection * placeDistance;
        placePosition.y = 0.15f;

        Quaternion placeRotation = Quaternion.LookRotation(placementDirection);

        Instantiate(claymorePrefab, placePosition, placeRotation);

        currentCooldown = placeCooldown;
    }

    // Этот метод выбирает направление установки клеймора.
    private Vector3 GetPlacementDirection()
    {
        // 1. Сначала пытаемся взять специальное направление клеймора.
        if (inputReader != null && inputReader.HasClaymoreAimDirection)
        {
            Vector3 claymoreDirection = inputReader.ClaymoreAimDirection;
            claymoreDirection.y = 0f;

            if (claymoreDirection.sqrMagnitude > 0.001f)
            {
                return claymoreDirection.normalized;
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