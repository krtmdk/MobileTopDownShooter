using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    // Визуальный фон стика.
    // По его размеру считаем направление и силу отклонения.

    [SerializeField] private RectTransform handle;
    // Ручка стика, которую двигаем внутри фона.

    [Header("Settings")]
    [SerializeField] private float handleRange = 60f;
    // Максимальное смещение ручки от центра.

    public Vector2 InputVector { get; private set; }
    // Текущее значение стика в диапазоне примерно от -1 до 1.

    private Canvas parentCanvas;
    // Родительский Canvas.

    private Camera uiCamera;
    // Камера UI.
    // Для Screen Space Overlay будет null, и это нормально.

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = parentCanvas.worldCamera;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Сразу обновляем положение ручки в момент первого нажатия.
        UpdateJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Пока палец или мышь двигается,
        // обновляем вектор и положение ручки.
        UpdateJoystick(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // При отпускании возвращаем всё в центр.
        InputVector = Vector2.zero;

        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }

    private void UpdateJoystick(PointerEventData eventData)
    {
        if (background == null || handle == null)
        {
            return;
        }

        // Переводим экранную позицию мыши/касания
        // в локальные координаты внутри background.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            uiCamera,
            out Vector2 localPoint))
        {
            // Берём РЕАЛЬНЫЙ размер прямоугольника background.
            // Важно: используем rect.size, а не sizeDelta,
            // потому что background может быть растянут якорями.
            Vector2 rectSize = background.rect.size;
            Vector2 halfSize = rectSize * 0.5f;

            if (halfSize.x <= 0f || halfSize.y <= 0f)
            {
                return;
            }

            // Нормализуем точку в диапазон примерно от -1 до 1.
            Vector2 normalized = new Vector2(
                localPoint.x / halfSize.x,
                localPoint.y / halfSize.y
            );

            // Ограничиваем длину вектора единицей,
            // чтобы ручка не выходила слишком далеко за пределы.
            InputVector = Vector2.ClampMagnitude(normalized, 1f);

            // Двигаем ручку внутри допустимого радиуса.
            handle.anchoredPosition = InputVector * handleRange;
        }
    }
}