using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyRiotCharge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    // Цель, на которую громила делает рывок

    [Header("Charge Settings")]
    [SerializeField] private float chargeRange = 6f;
    // Дистанция, на которой громила может начать рывок

    [SerializeField] private float chargeSpeed = 8.5f;
    // Скорость рывка

    [SerializeField] private float chargeDuration = 0.55f;
    // Длительность рывка

    [SerializeField] private float chargeCooldown = 4.5f;
    // Задержка между рывками

    [SerializeField] private float windUpTime = 0.55f;
    // Подготовка перед рывком

    [SerializeField] private float minChargeDistance = 2.2f;
    // Минимальная дистанция, чтобы громила не делал рывок в упор

    private NavMeshAgent agent;
    // NavMeshAgent громилы

    private float currentChargeCooldown;
    // Остаток кулдауна

    private bool isCharging;
    // Идёт ли рывок

    private bool isWindingUp;
    // Идёт ли подготовка к рывку

    private float currentChargeTimer;
    // Таймер рывка

    private float currentWindUpTimer;
    // Таймер подготовки

    private Vector3 chargeDirection;
    // Направление рывка

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        FindTargetIfNeeded();
    }

    private void Update()
    {
        FindTargetIfNeeded();
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

    private void FindTargetIfNeeded()
    {
        if (target != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    private void UpdateCooldown()
    {
        if (currentChargeCooldown <= 0f)
        {
            return;
        }

        currentChargeCooldown -= Time.deltaTime;

        if (currentChargeCooldown < 0f)
        {
            currentChargeCooldown = 0f;
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

        if (directionToTarget.sqrMagnitude <= 0.001f)
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

        if (!agent.enabled || !agent.isOnNavMesh)
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