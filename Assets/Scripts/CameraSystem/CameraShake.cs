using UnityEngine;

// Этот скрипт отвечает за тряску камеры.
// Его нужно повесить на Main Camera.
public class CameraShake : MonoBehaviour
{
    [Header("Default Shake Settings")]
    [SerializeField] private float defaultDuration = 0.15f;
    // Стандартная длительность тряски

    [SerializeField] private float defaultStrength = 0.12f;
    // Стандартная сила тряски

    private float currentDuration;
    // Сколько времени тряска ещё длится

    private float currentStrength;
    // Максимальная сила тряски

    private Vector3 shakeOffset;
    // Смещение камеры

    public Vector3 GetShakeOffset()
    {
        if (currentDuration <= 0f)
        {
            return Vector3.zero;
        }

        // Уменьшаем время
        currentDuration -= Time.deltaTime;

        // Плавное затухание силы
        float normalizedTime = currentDuration;
        float strength = currentStrength * normalizedTime;

        // Генерируем мягкую тряску
        float x = Random.Range(-1f, 1f) * strength;
        float z = Random.Range(-1f, 1f) * strength * 0.5f;

        shakeOffset = new Vector3(x, 0f, z);

        return shakeOffset;
    }

    // Обычная тряска
    public void Shake()
    {
        currentDuration = defaultDuration;
        currentStrength = defaultStrength;
    }

    // Тряска с параметрами
    public void Shake(float duration, float strength)
    {
        currentDuration = duration;
        currentStrength = strength;
    }
}