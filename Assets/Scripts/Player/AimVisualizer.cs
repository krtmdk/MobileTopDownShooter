using UnityEngine;

public class AimVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    // Точка, откуда начинается линия прицела.

    [SerializeField] private LineRenderer lineRenderer;
    // Компонент, который рисует линию.

    [SerializeField] private PlayerInputReader inputReader;
    // Ссылка на источник ввода.
    // Теперь логика показа линии зависит не от мыши напрямую,
    // а от общего input-слоя.

    [Header("Aim Line Settings")]
    [SerializeField] private float aimDistance = 6f;
    // Максимальная длина линии прицела.

    [SerializeField] private bool showOnlyWhileShooting = true;
    // Если true, линия видна только во время стрельбы.

    [SerializeField] private LayerMask hitMask = Physics.DefaultRaycastLayers;
    // Какие слои участвуют в raycast.

    [Header("Aim Line Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    // Цвет линии по умолчанию.

    [SerializeField] private Color enemyColor = Color.red;
    // Цвет линии, если луч попал во врага.

    private void Awake()
    {
        // Если LineRenderer не назначен вручную,
        // пробуем взять его с этого же объекта.
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Если inputReader не назначен вручную,
        // пробуем найти его на родителе.
        if (inputReader == null)
        {
            inputReader = GetComponentInParent<PlayerInputReader>();
        }
    }

    private void Update()
    {
        UpdateAimLine();
    }

    private void UpdateAimLine()
    {
        if (firePoint == null || lineRenderer == null)
        {
            return;
        }

        if (inputReader == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // Если линия должна показываться только во время стрельбы,
        // включаем её только при активном флаге IsShooting.
        if (showOnlyWhileShooting)
        {
            bool isShooting = inputReader.IsShooting;
            lineRenderer.enabled = isShooting;

            if (!isShooting)
            {
                return;
            }
        }
        else
        {
            lineRenderer.enabled = true;
        }

        Vector3 startPoint = firePoint.position;
        Vector3 direction = firePoint.forward;
        Vector3 endPoint = startPoint + direction * aimDistance;

        // Ставим обычный цвет по умолчанию.
        SetLineColor(defaultColor);

        // Пускаем луч вперёд от точки стрельбы.
        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, aimDistance, hitMask))
        {
            // Если луч во что-то попал, заканчиваем линию в точке попадания.
            endPoint = hit.point;

            // Ищем EnemyHealth не только на самом collider,
            // но и у родительских объектов.
            EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                SetLineColor(enemyColor);
            }
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    private void SetLineColor(Color color)
    {
        // Меняем цвет линии через LineRenderer.
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        // Некоторые материалы игнорируют startColor/endColor,
        // поэтому дополнительно меняем цвет материала.
        if (lineRenderer.material != null)
        {
            lineRenderer.material.color = color;
        }
    }
}