using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    // Цель, за которой враг должен идти

    [SerializeField] private Transform enemyVisual;
    // Визуальная модель врага

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.6f;
    // Скорость обычного преследования

    [SerializeField] private float stopDistance = 1.4f;
    // Дистанция остановки возле игрока

    [SerializeField] private float pathUpdateInterval = 0.25f;
    // Как часто враг обновляет путь

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 14f;
    // Скорость поворота модели врага

    [Header("NavMesh Safety")]
    [SerializeField] private float navMeshSearchRadius = 5f;
    // Радиус поиска ближайшего NavMesh возле врага

    [SerializeField] private float targetNavMeshSearchRadius = 3f;
    // Радиус поиска ближайшего NavMesh возле игрока

    private NavMeshAgent agent;
    // NavMeshAgent врага

    private EnemyRiotCharge riotCharge;
    // Скрипт рывка громилы, если он есть

    private float pathUpdateTimer;
    // Таймер обновления пути

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        riotCharge = GetComponent<EnemyRiotCharge>();

        FindTargetIfNeeded();
        SetupAgent();
    }

    private void Start()
    {
        EnsureAgentIsOnNavMesh();
    }

    private void Update()
    {
        FindTargetIfNeeded();

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
            EnsureAgentIsOnNavMesh();
            return;
        }

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
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.autoBraking = true;
        agent.autoRepath = true;
        agent.avoidancePriority = Random.Range(20, 80);
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

    private void EnsureAgentIsOnNavMesh()
    {
        if (agent == null)
        {
            return;
        }

        if (!agent.enabled)
        {
            return;
        }

        if (agent.isOnNavMesh)
        {
            return;
        }

        NavMeshHit hit;

        bool foundPosition = NavMesh.SamplePosition(
            transform.position,
            out hit,
            navMeshSearchRadius,
            NavMesh.AllAreas
        );

        if (!foundPosition)
        {
            Debug.LogWarning("Enemy cannot find NavMesh near spawn: " + gameObject.name);
            return;
        }

        agent.Warp(hit.position);
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

        if (!agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        NavMeshHit targetHit;

        bool foundTargetPoint = NavMesh.SamplePosition(
            target.position,
            out targetHit,
            targetNavMeshSearchRadius,
            NavMesh.AllAreas
        );

        if (!foundTargetPoint)
        {
            return;
        }

        NavMeshPath path = new NavMeshPath();

        bool pathCalculated = agent.CalculatePath(targetHit.position, path);

        if (!pathCalculated)
        {
            return;
        }

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            return;
        }

        agent.SetDestination(targetHit.position);
    }

    private void StopAgentMovement()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = true;
        agent.ResetPath();
    }

    private void ResumeAgentMovement()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
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
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return 0f;
        }

        Vector3 velocity = agent.velocity;
        velocity.y = 0f;

        return velocity.magnitude;
    }
}