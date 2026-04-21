using UnityEngine;
using UnityEngine.EventSystems;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button State")]
    [SerializeField] private bool isHoldButton = false;
    // Если true — кнопка считается удерживаемой.
    // Если false — кнопка работает как "нажал / отпустил".

    public bool IsPressed { get; private set; }
    // Кнопка сейчас удерживается.

    public bool WasPressedThisFrame { get; private set; }
    // Кнопка была нажата в этом кадре.

    public bool WasReleasedThisFrame { get; private set; }
    // Кнопка была отпущена в этом кадре.

    private void LateUpdate()
    {
        // Одноразовые флаги живут только один кадр.
        WasPressedThisFrame = false;
        WasReleasedThisFrame = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        WasPressedThisFrame = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        WasReleasedThisFrame = true;
    }

    public bool GetButtonValue()
    {
        // Для удерживаемой кнопки возвращаем текущее состояние удержания.
        if (isHoldButton)
        {
            return IsPressed;
        }

        // Для обычной кнопки возвращаем факт нажатия в этом кадре.
        return WasPressedThisFrame;
    }

    public bool GetReleaseValue()
    {
        // Возвращаем факт отпускания кнопки в этом кадре.
        return WasReleasedThisFrame;
    }
}