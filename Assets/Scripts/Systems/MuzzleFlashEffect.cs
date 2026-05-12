using UnityEngine;

// Этот скрипт быстро включает вспышку выстрела,
// а затем выключает объект обратно.
public class MuzzleFlashEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light flashLight;
    // Свет вспышки


    [Header("Settings")]
    [SerializeField] private float flashDuration = 0.05f;
    // Сколько секунд длится вспышка

    private float flashTimer;
    // Таймер до выключения вспышки

    private void Awake()
    {
        if (flashLight == null)
        {
            flashLight = GetComponentInChildren<Light>();
        }



        SetVisible(false);
    }

    private void Update()
    {
        if (flashTimer <= 0f)
        {
            return;
        }

        flashTimer -= Time.deltaTime;

        if (flashTimer <= 0f)
        {
            SetVisible(false);
        }
    }

    public void Play()
    {
        flashTimer = flashDuration;

        SetVisible(true);

    }

    private void SetVisible(bool value)
    {
        if (flashLight != null)
        {
            flashLight.enabled = value;
        }

    }
}