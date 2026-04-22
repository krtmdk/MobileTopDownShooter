using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerVisual;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Animator playerAnimator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 12f;

    private Rigidbody rb;
    private Vector3 movementDirection;

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
        movementDirection = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    private void Move()
    {
        // Если Rigidbody уже переведён в kinematic,
        // ничего больше с velocity не делаем.
        if (rb == null || rb.isKinematic)
        {
            return;
        }

        Vector3 targetVelocity = movementDirection * moveSpeed;
        Vector3 currentVelocity = rb.velocity;

        rb.velocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
    }

    private void RotateVisual()
    {
        if (playerVisual == null || inputReader == null)
        {
            return;
        }

        Vector3 aimDirection = inputReader.AimDirection;
        aimDirection.y = 0f;

        if (aimDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

        playerVisual.rotation = Quaternion.Slerp(
            playerVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
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