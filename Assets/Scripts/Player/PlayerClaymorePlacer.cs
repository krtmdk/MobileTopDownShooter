using UnityEngine;

// Этот скрипт отвечает за установку клеймора.
// Теперь клейморы ограничены по количеству.
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

    [Header("Ammo Settings")]
    [SerializeField] private int startClaymores = 1;
    // Сколько клейморов у игрока в начале игры.

    [SerializeField] private int maxClaymores = 2;
    // Максимум клейморов, который игрок может носить.

    private int currentClaymores;
    // Текущее количество клейморов.

    private float currentCooldown;
    // Остаток кулдауна.

    private void Awake()
    {
        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        currentClaymores = Mathf.Clamp(startClaymores, 0, maxClaymores);
    }

    private void Update()
    {
        UpdateCooldown();
        HandleClaymoreInput();
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

        // Если клейморов нет, установку не выполняем.
        if (currentClaymores <= 0)
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

        // Тратим один клеймор после успешной установки.
        currentClaymores--;

        currentCooldown = placeCooldown;
    }

    private Vector3 GetPlacementDirection()
    {
        if (inputReader != null && inputReader.HasClaymoreAimDirection)
        {
            Vector3 claymoreDirection = inputReader.ClaymoreAimDirection;
            claymoreDirection.y = 0f;

            if (claymoreDirection.sqrMagnitude > 0.001f)
            {
                return claymoreDirection.normalized;
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

    public int GetCurrentClaymores()
    {
        return currentClaymores;
    }

    public int GetMaxClaymores()
    {
        return maxClaymores;
    }

    public void AddClaymores(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentClaymores += amount;

        if (currentClaymores > maxClaymores)
        {
            currentClaymores = maxClaymores;
        }
    }
}