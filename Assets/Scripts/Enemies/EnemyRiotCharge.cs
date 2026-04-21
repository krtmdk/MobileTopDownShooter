using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyRiotCharge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    // Цель, на которую громила будет делать рывок.

    [Header("Charge Settings")]
    [SerializeField] private float chargeRange = 6f;
    // Дистанция, на которой громила может начать рывок.

    [SerializeField] private float chargeSpeed = 10f;
    // Скорость рывка.

    [SerializeField] private float chargeDuration = 0.6f;
    // Длительность самого рывка.

    [SerializeField] private float chargeCooldown = 4f;
    // Задержка между рывками.

    [SerializeField] private float windUpTime = 0.5f;
    // Время подготовки перед рывком.
    // Это и есть телеграф: громила замирает и готовится.

    private Rigidbody rb;
    // Rigidbody громилы.

    private float currentChargeCooldown;
    // Остаток кулдауна до следующего рывка.

    private bool isCharging;
    // Сейчас идёт активный рывок.

    private bool isWindingUp;
    // Сейчас идёт подготовка к рывку.

    private float currentChargeTimer;
    // Таймер рывка.

    private float currentWindUpTimer;
    // Таймер подготовки.

    private Vector3 chargeDirection;
    // Направление рывка.

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Если цель вручную не назначена, ищем игрока по тегу.
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        UpdateCooldown();

        if (isWindingUp)
        {
            UpdateWindUp();
            return;
        }

        if (isCharging)
        {
            UpdateCharge();
            return;
        }

        TryStartChargeWindUp();
    }

    private void FixedUpdate()
    {
        if (isCharging)
        {
            ApplyChargeMovement();
        }
    }

    private void UpdateCooldown()
    {
        if (currentChargeCooldown > 0f)
        {
            currentChargeCooldown -= Time.deltaTime;

            if (currentChargeCooldown < 0f)
            {
                currentChargeCooldown = 0f;
            }
        }
    }

    private void TryStartChargeWindUp()
    {
        // Если кулдаун ещё не прошёл, ничего не делаем.
        if (currentChargeCooldown > 0f)
        {
            return;
        }

        if (target == null)
        {
            return;
        }

        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;

        float distanceToTarget = directionToTarget.magnitude;

        // Слишком далеко — не начинаем.
        if (distanceToTarget > chargeRange)
        {
            return;
        }

        // Слишком близко — тоже не начинаем.
        if (distanceToTarget < 2f)
        {
            return;
        }

        // Запоминаем направление будущего рывка.
        chargeDirection = directionToTarget.normalized;

        // Включаем стадию подготовки.
        isWindingUp = true;
        currentWindUpTimer = windUpTime;

        // Кулдаун запускаем уже сейчас,
        // чтобы не было повторного старта подготовки.
        currentChargeCooldown = chargeCooldown;
    }

    private void UpdateWindUp()
    {
        currentWindUpTimer -= Time.deltaTime;

        if (currentWindUpTimer > 0f)
        {
            return;
        }

        StartCharge();
    }

    private void StartCharge()
    {
        isWindingUp = false;
        isCharging = true;
        currentChargeTimer = chargeDuration;
    }

    private void UpdateCharge()
    {
        currentChargeTimer -= Time.deltaTime;

        if (currentChargeTimer <= 0f)
        {
            StopCharge();
        }
    }

    private void ApplyChargeMovement()
    {
        Vector3 currentVelocity = rb.velocity;

        rb.velocity = new Vector3(
            chargeDirection.x * chargeSpeed,
            currentVelocity.y,
            chargeDirection.z * chargeSpeed
        );
    }

    private void StopCharge()
    {
        isCharging = false;
    }

    public bool IsCharging()
    {
        return isCharging;
    }

    public bool IsWindingUp()
    {
        return isWindingUp;
    }
}