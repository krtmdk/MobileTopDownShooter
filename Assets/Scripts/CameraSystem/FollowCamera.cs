using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    // Цель, за которой должна следовать камера.
    // В нашем случае это объект Player.

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -8f);
    // Смещение камеры относительно игрока.
    // Например: выше по Y и немного сзади по Z.

    [SerializeField] private float smoothTime = 0.15f;
    // Время сглаживания.
    // Чем меньше значение, тем резче камера следует за игроком.
    // Чем больше значение, тем плавнее камера догоняет цель.

    private Vector3 currentVelocity;
    // Вспомогательный вектор, который использует SmoothDamp.
    // Нужен для внутреннего расчёта плавного движения камеры.

    private void LateUpdate()
    {
        // LateUpdate используем потому, что к этому моменту
        // игрок уже успел обновить своё движение.
        // Камера берёт уже актуальную позицию цели.

        if (target == null)
        {
            return;
        }

        // Вычисляем позицию, в которой камера должна быть.
        Vector3 targetPosition = target.position + offset;

        // Плавно двигаем камеру к целевой позиции.
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );
    }
}