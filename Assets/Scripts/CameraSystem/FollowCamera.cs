using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    // Игрок, за которым следует камера

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, 0f);
    // Базовое смещение камеры относительно игрока

    [SerializeField] private float smoothTime = 0.2f;
    // Плавность движения камеры

    [Header("Input")]
    [SerializeField] private PlayerInputReader inputReader;
    // Ссылка на ввод игрока

    [Header("Offset Settings")]
    [SerializeField] private float aimOffsetStrength = 4f;
    // Смещение камеры во время стрельбы

    [SerializeField] private float moveOffsetStrength = 2f;
    // Смещение камеры во время движения

    [Header("Shake")]
    [SerializeField] private CameraShake cameraShake;
    // Скрипт тряски камеры

    private Vector3 currentVelocity;
    // Вспомогательный вектор для SmoothDamp

    private void Awake()
    {
        if (cameraShake == null)
        {
            cameraShake = GetComponent<CameraShake>();
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position + offset;

        Vector3 offsetDirection = Vector3.zero;

        if (inputReader != null)
        {
            if (inputReader.IsShooting)
            {
                Vector3 aimDir = inputReader.AimDirection;
                aimDir.y = 0f;

                if (aimDir.sqrMagnitude > 0.001f)
                {
                    aimDir.Normalize();
                    offsetDirection = aimDir * aimOffsetStrength;
                }
            }
            else
            {
                Vector2 moveInput = inputReader.MoveInput;

                if (moveInput.sqrMagnitude > 0.001f)
                {
                    Vector3 camForward = transform.forward;
                    Vector3 camRight = transform.right;

                    camForward.y = 0f;
                    camRight.y = 0f;

                    camForward.Normalize();
                    camRight.Normalize();

                    Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

                    offsetDirection = moveDir.normalized * moveOffsetStrength;
                }
            }
        }

        targetPosition += offsetDirection;

        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );

        if (cameraShake != null)
        {
            smoothedPosition += cameraShake.GetShakeOffset();
        }

        transform.position = smoothedPosition;
    }
}