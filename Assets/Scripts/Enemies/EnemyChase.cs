using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    // Цель, за которой враг должен идти.
    // Если вручную не назначена, враг попытается найти игрока автоматически.

    [SerializeField] private Transform enemyVisual;
    // Визуальная часть врага.
    // Её вращаем отдельно от физического корня.

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    // Скорость движения врага.

    [SerializeField] private float stopDistance = 1.2f;
    // Дистанция, на которой враг перестаёт приближаться к игроку.

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    // Скорость поворота визуала врага.

    [Header("Obstacle Avoidance Settings")]
    [SerializeField] private float obstacleCheckDistance = 1.2f;
    // На какую дистанцию враг проверяет препятствие прямо перед собой.

    [SerializeField] private float sideCheckDistance = 1f;
    // На какую дистанцию враг проверяет свободное место справа и слева.

    [SerializeField] private float obstacleCheckRadius = 0.25f;
    // Радиус SphereCast для проверки препятствий.

    [SerializeField] private float sideAvoidanceWeight = 0.9f;
    // Насколько сильно враг смещает направление в сторону обхода.

    [SerializeField] private float avoidanceLockDuration = 0.6f;
    // Сколько времени враг удерживает выбранное направление обхода,
    // чтобы не дёргаться каждый кадр у углов.

    [SerializeField] private LayerMask obstacleMask = Physics.DefaultRaycastLayers;
    // Какие слои считаются препятствиями.

    private Rigidbody rb;
    // Rigidbody физического корня врага.

    private Vector3 moveDirection;
    // Итоговое направление движения врага.

    private EnemyRiotCharge riotCharge;
    // Ссылка на рывок громилы, если он есть.

    private Vector3 lockedAvoidanceDirection;
    // Запомненное направление обхода препятствия.

    private float currentAvoidanceLockTimer;
    // Таймер удержания выбранного направления обхода.

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Если ссылка на цель не назначена вручную,
        // пробуем найти объект с тегом Player.
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        riotCharge = GetComponent<EnemyRiotCharge>();
    }

    private void Update()
    {
        UpdateAvoidanceTimer();
        UpdateDirection();
        RotateVisual();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void UpdateAvoidanceTimer()
    {
        if (currentAvoidanceLockTimer > 0f)
        {
            currentAvoidanceLockTimer -= Time.deltaTime;

            if (currentAvoidanceLockTimer < 0f)
            {
                currentAvoidanceLockTimer = 0f;
            }
        }
    }

    private void UpdateDirection()
    {
        if (target == null)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;

        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= stopDistance)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 desiredDirection = directionToTarget.normalized;

        moveDirection = GetAvoidedDirection(desiredDirection);
    }

    private Vector3 GetAvoidedDirection(Vector3 desiredDirection)
    {
        if (desiredDirection.sqrMagnitude <= 0.001f)
        {
            return Vector3.zero;
        }

        Vector3 castOrigin = transform.position + Vector3.up * 0.35f;

        bool isBlockedForward = Physics.SphereCast(
            castOrigin,
            obstacleCheckRadius,
            desiredDirection,
            out _,
            obstacleCheckDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        // Если путь вперёд свободен, сбрасываем обход и идём прямо к игроку.
        if (!isBlockedForward)
        {
            lockedAvoidanceDirection = Vector3.zero;
            currentAvoidanceLockTimer = 0f;
            return desiredDirection;
        }

        // Если ранее уже выбрали направление обхода и таймер ещё жив,
        // продолжаем идти в эту сторону, чтобы не дёргаться каждый кадр.
        if (currentAvoidanceLockTimer > 0f && lockedAvoidanceDirection.sqrMagnitude > 0.001f)
        {
            return (desiredDirection + lockedAvoidanceDirection * sideAvoidanceWeight).normalized;
        }

        Vector3 rightDirection = Vector3.Cross(Vector3.up, desiredDirection).normalized;
        Vector3 leftDirection = -rightDirection;

        bool isBlockedRight = Physics.SphereCast(
            castOrigin,
            obstacleCheckRadius,
            rightDirection,
            out _,
            sideCheckDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        bool isBlockedLeft = Physics.SphereCast(
            castOrigin,
            obstacleCheckRadius,
            leftDirection,
            out _,
            sideCheckDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        // Если справа свободно, а слева нет — выбираем правый обход.
        if (!isBlockedRight && isBlockedLeft)
        {
            lockedAvoidanceDirection = rightDirection;
            currentAvoidanceLockTimer = avoidanceLockDuration;
            return (desiredDirection + rightDirection * sideAvoidanceWeight).normalized;
        }

        // Если слева свободно, а справа нет — выбираем левый обход.
        if (!isBlockedLeft && isBlockedRight)
        {
            lockedAvoidanceDirection = leftDirection;
            currentAvoidanceLockTimer = avoidanceLockDuration;
            return (desiredDirection + leftDirection * sideAvoidanceWeight).normalized;
        }

        // Если обе стороны свободны, временно выбираем правую сторону.
        // Это простой и стабильный вариант без лишней случайности.
        if (!isBlockedRight && !isBlockedLeft)
        {
            lockedAvoidanceDirection = rightDirection;
            currentAvoidanceLockTimer = avoidanceLockDuration;
            return (desiredDirection + rightDirection * sideAvoidanceWeight).normalized;
        }

        // Если всё заблокировано, сохраняем ноль.
        lockedAvoidanceDirection = Vector3.zero;
        currentAvoidanceLockTimer = 0f;

        return Vector3.zero;
    }

    private void Move()
    {
        // Если враг сейчас делает рывок или готовится к нему,
        // обычное движение отключаем.
        if (riotCharge != null && (riotCharge.IsCharging() || riotCharge.IsWindingUp()))
        {
            return;
        }

        Vector3 targetVelocity = moveDirection * moveSpeed;
        Vector3 currentVelocity = rb.velocity;

        rb.velocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
    }

    private void RotateVisual()
    {
        if (enemyVisual == null)
        {
            return;
        }

        if (moveDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        enemyVisual.rotation = Quaternion.Slerp(
            enemyVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 forwardDirection = Application.isPlaying ? moveDirection : transform.forward;
        forwardDirection.y = 0f;

        if (forwardDirection.sqrMagnitude <= 0.001f)
        {
            forwardDirection = transform.forward;
            forwardDirection.y = 0f;
        }

        forwardDirection.Normalize();

        Vector3 castOrigin = transform.position + Vector3.up * 0.35f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(castOrigin, castOrigin + forwardDirection * obstacleCheckDistance);
        Gizmos.DrawWireSphere(castOrigin + forwardDirection * obstacleCheckDistance, obstacleCheckRadius);

        Vector3 rightDirection = Vector3.Cross(Vector3.up, forwardDirection).normalized;
        Vector3 leftDirection = -rightDirection;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(castOrigin, castOrigin + rightDirection * sideCheckDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(castOrigin, castOrigin + leftDirection * sideCheckDistance);
    }
}