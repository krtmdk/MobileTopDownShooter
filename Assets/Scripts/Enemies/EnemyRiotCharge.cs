using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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

    [SerializeField] private float minChargeDistance = 2f;
    // Минимальная дистанция до игрока, чтобы громила не делал рывок в упор.

    private NavMeshAgent agent;
    // NavMeshAgent громилы. Через него теперь двигаем рывок.

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
        agent = GetComponent<NavMeshAgent>();

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

        if (distanceToTarget > chargeRange)
        {
            return;
        }

        if (distanceToTarget < minChargeDistance)
        {
            return;
        }

        chargeDirection = directionToTarget.normalized;

        isWindingUp = true;
        currentWindUpTimer = windUpTime;

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

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    private void UpdateCharge()
    {
        currentChargeTimer -= Time.deltaTime;

        ApplyChargeMovement();

        if (currentChargeTimer <= 0f)
        {
            StopCharge();
        }
    }

    private void ApplyChargeMovement()
    {
        if (agent == null)
        {
            return;
        }

        if (!agent.enabled)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        Vector3 movement = chargeDirection * chargeSpeed * Time.deltaTime;

        agent.Move(movement);
    }

    private void StopCharge()
    {
        isCharging = false;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
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