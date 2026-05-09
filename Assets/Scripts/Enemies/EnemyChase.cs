using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    // Цель, за которой враг должен идти.
    // Обычно это Player.

    [SerializeField] private Transform enemyVisual;
    // Визуальная часть врага.
    // Её поворачиваем отдельно, чтобы модель смотрела по движению.

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    // Скорость движения врага через NavMeshAgent.

    [SerializeField] private float stopDistance = 1.2f;
    // Дистанция, на которой враг перестаёт подходить к игроку.

    [SerializeField] private float pathUpdateInterval = 0.15f;
    // Как часто враг обновляет путь к игроку.
    // Не нужно обновлять путь каждый кадр, это лишняя нагрузка.

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    // Скорость поворота визуальной модели врага.

    private NavMeshAgent agent;
    // Компонент NavMeshAgent, который строит путь по NavMesh.

    private EnemyRiotCharge riotCharge;
    // Скрипт рывка громилы, если он есть.

    private float pathUpdateTimer;
    // Таймер до следующего обновления пути.

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        riotCharge = GetComponent<EnemyRiotCharge>();

        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        SetupAgent();
    }

    private void Update()
    {
        if (target == null || agent == null)
        {
            return;
        }

        // Если громила готовится к рывку или уже делает рывок,
        // обычное движение NavMeshAgent временно отключаем.
        if (riotCharge != null && (riotCharge.IsCharging() || riotCharge.IsWindingUp()))
        {
            StopAgentMovement();
            RotateVisualByVelocity();
            return;
        }

        ResumeAgentMovement();
        UpdatePathTimer();
        RotateVisualByVelocity();
    }

    private void SetupAgent()
    {
        if (agent == null)
        {
            return;
        }

        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance;

        // Мы сами поворачиваем визуал, поэтому агенту не даём крутить объект.
        agent.updateRotation = false;

        // Позицию агент обновляет сам.
        agent.updatePosition = true;

        // Автоторможение помогает врагу не перелетать точку остановки.
        agent.autoBraking = true;
    }

    private void UpdatePathTimer()
    {
        pathUpdateTimer -= Time.deltaTime;

        if (pathUpdateTimer > 0f)
        {
            return;
        }

        pathUpdateTimer = pathUpdateInterval;
        UpdateDestination();
    }

    private void UpdateDestination()
    {
        if (target == null || agent == null)
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

        agent.SetDestination(target.position);
    }

    private void StopAgentMovement()
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

        agent.isStopped = true;
        agent.ResetPath();
    }

    private void ResumeAgentMovement()
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

        agent.isStopped = false;
    }

    private void RotateVisualByVelocity()
    {
        if (enemyVisual == null || agent == null)
        {
            return;
        }

        Vector3 moveDirection = agent.velocity;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);

        enemyVisual.rotation = Quaternion.Slerp(
            enemyVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public float GetCurrentSpeed()
    {
        if (agent == null)
        {
            return 0f;
        }

        Vector3 velocity = agent.velocity;
        velocity.y = 0f;

        return velocity.magnitude;
    }
}