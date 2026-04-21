using UnityEngine;

// Этот атрибут говорит Unity:
// на объекте со скриптом обязательно должен быть Rigidbody.
// Если его нет, Unity покажет ошибку конфигурации.
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerVisual;
    // Визуальная часть игрока.
    // Поворачиваем именно её, а не физический корень Player.

    [SerializeField] private PlayerInputReader inputReader;
    // Ссылка на скрипт, который читает ввод.
    // Теперь PlayerMovement сам не работает напрямую с клавиатурой и мышью.

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    // Скорость движения игрока по плоскости.

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 12f;
    // Скорость поворота визуала в сторону прицеливания.

    private Rigidbody rb;
    // Rigidbody корневого объекта Player.

    private Vector3 movementDirection;
    // Направление движения по плоскости XZ.

    private void Awake()
    {
        // Сохраняем ссылку на Rigidbody один раз.
        rb = GetComponent<Rigidbody>();

        // Если ссылка на inputReader не назначена вручную,
        // пробуем найти его на этом же объекте.
        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }
    }

    private void Update()
    {
        // Читаем уже готовый ввод из PlayerInputReader.
        ReadInputFromReader();

        // Поворачиваем визуал в сторону прицеливания.
        RotateVisual();
    }

    private void FixedUpdate()
    {
        // Всё, что связано с физикой и Rigidbody, делаем в FixedUpdate.
        Move();
    }

    private void ReadInputFromReader()
    {
        // Если inputReader не найден, останавливаем движение.
        if (inputReader == null)
        {
            movementDirection = Vector3.zero;
            return;
        }

        // Берём вектор движения из inputReader.
        Vector2 moveInput = inputReader.MoveInput;

        // Переводим его в 3D-направление по плоскости XZ.
        movementDirection = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    private void Move()
    {
        // Вычисляем желаемую скорость по плоскости.
        Vector3 targetVelocity = movementDirection * moveSpeed;

        // Берём текущую скорость Rigidbody.
        Vector3 currentVelocity = rb.velocity;

        // Меняем только X и Z.
        // Y оставляем как есть, чтобы не ломать вертикальную физику.
        rb.velocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
    }

    private void RotateVisual()
    {
        // Если не назначен визуал, поворачивать нечего.
        if (playerVisual == null)
        {
            return;
        }

        // Если нет inputReader, не можем получить направление прицеливания.
        if (inputReader == null)
        {
            return;
        }

        // Берём уже готовое направление прицеливания.
        Vector3 aimDirection = inputReader.AimDirection;
        aimDirection.y = 0f;

        // Если направление слишком маленькое,
        // не пытаемся поворачивать игрока.
        if (aimDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        // Создаём поворот в сторону прицеливания.
        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

        // Плавно поворачиваем визуал.
        playerVisual.rotation = Quaternion.Slerp(
            playerVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}