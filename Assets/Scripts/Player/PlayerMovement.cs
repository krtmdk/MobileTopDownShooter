using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private enum RotationMode
    {
        AimDirectionAlways,
        ShootAimOtherwiseMove
    }

    [Header("References")]
    [SerializeField] private Transform playerVisual;
    // Визуальная модель игрока, которую нужно поворачивать

    [SerializeField] private PlayerInputReader inputReader;
    // Скрипт, который хранит ввод игрока

    [SerializeField] private Animator playerAnimator;
    // Animator игрока

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    // Скорость движения игрока

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 12f;
    // Скорость поворота модели игрока

    [SerializeField] private RotationMode rotationMode = RotationMode.AimDirectionAlways;
    // AimDirectionAlways — режим для ПК, мышь всегда управляет направлением модели
    // ShootAimOtherwiseMove — режим для мобильной версии

    private Rigidbody rb;
    // Rigidbody игрока

    private Vector3 movementDirection;
    // Направление движения игрока

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }
    }

    private void Update()
    {
        ReadInputFromReader();
        RotateVisual();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ReadInputFromReader()
    {
        if (inputReader == null)
        {
            movementDirection = Vector3.zero;
            return;
        }

        Vector2 moveInput = inputReader.MoveInput;

        movementDirection = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );
    }

    private void Move()
    {
        if (rb == null || rb.isKinematic)
        {
            return;
        }

        Vector3 targetVelocity = movementDirection * moveSpeed;
        Vector3 currentVelocity = rb.velocity;

        rb.velocity = new Vector3(
            targetVelocity.x,
            currentVelocity.y,
            targetVelocity.z
        );
    }

    private void RotateVisual()
    {
        if (playerVisual == null || inputReader == null)
        {
            return;
        }

        Vector3 lookDirection = GetLookDirection();

        if (lookDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        playerVisual.rotation = Quaternion.Slerp(
            playerVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private Vector3 GetLookDirection()
    {
        if (rotationMode == RotationMode.AimDirectionAlways)
        {
            Vector3 aimDirection = inputReader.AimDirection;
            aimDirection.y = 0f;

            if (aimDirection.sqrMagnitude > 0.001f)
            {
                return aimDirection.normalized;
            }

            if (movementDirection.sqrMagnitude > 0.001f)
            {
                return movementDirection.normalized;
            }

            return Vector3.zero;
        }

        if (rotationMode == RotationMode.ShootAimOtherwiseMove)
        {
            if (inputReader.IsShooting)
            {
                Vector3 aimDirection = inputReader.AimDirection;
                aimDirection.y = 0f;

                if (aimDirection.sqrMagnitude > 0.001f)
                {
                    return aimDirection.normalized;
                }
            }

            if (movementDirection.sqrMagnitude > 0.001f)
            {
                return movementDirection.normalized;
            }

            return Vector3.zero;
        }

        return Vector3.zero;
    }

    private void UpdateAnimator()
    {
        if (playerAnimator == null || rb == null)
        {
            return;
        }

        Vector3 velocity = rb.velocity;
        velocity.y = 0f;

        float speed = velocity.magnitude;
        bool isMoving = speed > 0.25f;

        playerAnimator.SetFloat("Speed", speed);
        playerAnimator.SetBool("IsMoving", isMoving);
    }
}