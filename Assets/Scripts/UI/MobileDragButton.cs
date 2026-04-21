using UnityEngine;
using UnityEngine.EventSystems;

// Этот скрипт превращает обычную UI-кнопку в drag-кнопку.
// Кнопку можно зажать, потянуть в сторону и отпустить.
// При этом сама кнопка будет немного смещаться как мини-стик.
public class MobileDragButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private float maxDragDistance = 45f;
    // Максимальная дистанция, на которую можно сместить кнопку от центра.

    public Vector2 DragInput { get; private set; }
    // Нормализованное направление drag.
    // Значения примерно от -1 до 1.

    public bool IsDragging { get; private set; }
    // Кнопка сейчас удерживается.

    public bool WasReleasedThisFrame { get; private set; }
    // Кнопка была отпущена в этом кадре.

    private RectTransform rectTransform;
    // RectTransform этой кнопки.

    private Canvas parentCanvas;
    // Родительский Canvas.

    private Camera uiCamera;
    // Камера UI.
    // Для Screen Space Overlay будет null, и это нормально.

    private Vector2 startAnchoredPosition;
    // Исходная позиция кнопки.

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            startAnchoredPosition = rectTransform.anchoredPosition;
        }

        parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = parentCanvas.worldCamera;
        }
    }

    private void LateUpdate()
    {
        // Флаг отпускания живёт только один кадр.
        WasReleasedThisFrame = false;
    }

    // Этот метод вызывается при первом нажатии на кнопку.
    public void OnPointerDown(PointerEventData eventData)
    {
        IsDragging = true;
        UpdateDrag(eventData);
    }

    // Этот метод вызывается, пока палец или мышь двигаются.
    public void OnDrag(PointerEventData eventData)
    {
        UpdateDrag(eventData);
    }

    // Этот метод вызывается, когда кнопку отпускают.
    public void OnPointerUp(PointerEventData eventData)
    {
        IsDragging = false;
        WasReleasedThisFrame = true;

        // Сбрасываем направление.
        DragInput = Vector2.zero;

        // Возвращаем кнопку в исходную позицию.
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startAnchoredPosition;
        }
    }

    // Этот метод переводит позицию пальца в направление drag
    // и двигает саму кнопку в пределах ограниченного радиуса.
    private void UpdateDrag(PointerEventData eventData)
    {
        if (rectTransform == null)
        {
            return;
        }

        RectTransform parentRect = rectTransform.parent as RectTransform;

        if (parentRect == null)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            uiCamera,
            out Vector2 localPoint))
        {
            // Считаем смещение пальца относительно исходной позиции кнопки.
            Vector2 offset = localPoint - startAnchoredPosition;

            // Ограничиваем радиус смещения.
            Vector2 clampedOffset = Vector2.ClampMagnitude(offset, maxDragDistance);

            // Получаем нормализованное направление.
            DragInput = clampedOffset / maxDragDistance;

            // Смещаем саму кнопку.
            rectTransform.anchoredPosition = startAnchoredPosition + clampedOffset;
        }
    }

    // Этот метод проверяет, достаточно ли сильно игрок потянул кнопку.
    public bool HasMeaningfulDrag(float threshold = 0.2f)
    {
        return DragInput.magnitude >= threshold;
    }
}