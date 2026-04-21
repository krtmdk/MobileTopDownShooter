using UnityEngine;

// Этот скрипт показывает линию направления гранаты,
// пока игрок удерживает кнопку гранаты.
// Цвет, длину и толщину линии можно менять из Inspector.
public class GrenadeAimVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform originPoint;
    // Точка, откуда начинается линия направления.

    [SerializeField] private LineRenderer lineRenderer;
    // Компонент линии.

    [SerializeField] private PlayerInputReader inputReader;
    // Источник ввода.

    [Header("Line Settings")]
    [SerializeField] private float lineLength = 3f;
    // Длина линии направления.

    [SerializeField] private float startWidth = 0.08f;
    // Толщина линии в начале.

    [SerializeField] private float endWidth = 0.04f;
    // Толщина линии в конце.

    [Header("Line Colors")]
    [SerializeField] private Color startColor = new Color(1f, 0.85f, 0.2f, 1f);
    // Цвет линии в начале.
    // По умолчанию жёлто-оранжевый цвет для гранаты.

    [SerializeField] private Color endColor = new Color(1f, 0.45f, 0.1f, 1f);
    // Цвет линии в конце.

    private void Awake()
    {
        // Если LineRenderer не назначен вручную,
        // пробуем взять его с этого же объекта.
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Если inputReader не назначен вручную,
        // пробуем найти его у родителя.
        if (inputReader == null)
        {
            inputReader = GetComponentInParent<PlayerInputReader>();
        }
    }

    private void Update()
    {
        UpdateLine();
    }

    // Этот метод каждый кадр обновляет линию направления гранаты.
    private void UpdateLine()
    {
        if (originPoint == null || lineRenderer == null || inputReader == null)
        {
            return;
        }

        // Если кнопку гранаты не удерживают, линию скрываем.
        if (!inputReader.GrenadePressed)
        {
            lineRenderer.enabled = false;
            return;
        }

        // Если пока нет валидного направления, линию тоже скрываем.
        if (!inputReader.HasGrenadeAimDirection)
        {
            lineRenderer.enabled = false;
            return;
        }

        Vector3 direction = inputReader.GrenadeAimDirection;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            lineRenderer.enabled = false;
            return;
        }

        direction.Normalize();

        // Включаем линию и применяем её визуальные настройки.
        lineRenderer.enabled = true;
        ApplyVisualSettings();

        Vector3 startPoint = originPoint.position;
        Vector3 endPoint = startPoint + direction * lineLength;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    // Этот метод применяет визуальные настройки линии.
    // Благодаря этому можно менять цвет и толщину прямо в Inspector.
    private void ApplyVisualSettings()
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;

        // Некоторые материалы LineRenderer игнорируют startColor / endColor.
        // Поэтому дополнительно можно покрасить материал в стартовый цвет.
        if (lineRenderer.material != null)
        {
            lineRenderer.material.color = startColor;
        }
    }
}