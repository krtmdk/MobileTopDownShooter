using UnityEngine;

// Этот скрипт отвечает за короткий визуальный эффект взрыва.
// Объект сам удаляется после завершения эффекта.
public class ExplosionEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light explosionLight;
    // Свет вспышки взрыва

    [SerializeField] private ParticleSystem explosionParticles;
    // Частицы взрыва

    [Header("Settings")]
    [SerializeField] private float lifeTime = 1f;
    // Через сколько секунд удалить объект эффекта

    [SerializeField] private float lightDuration = 0.12f;
    // Сколько секунд свет остаётся включённым

    private float lightTimer;
    // Таймер выключения света

    private void Awake()
    {
        if (explosionLight == null)
        {
            explosionLight = GetComponentInChildren<Light>();
        }

        if (explosionParticles == null)
        {
            explosionParticles = GetComponentInChildren<ParticleSystem>();
        }
    }

    private void Start()
    {
        lightTimer = lightDuration;

        if (explosionLight != null)
        {
            explosionLight.enabled = true;
        }

        if (explosionParticles != null)
        {
            explosionParticles.Play();
        }

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (explosionLight == null)
        {
            return;
        }

        if (lightTimer <= 0f)
        {
            return;
        }

        lightTimer -= Time.deltaTime;

        if (lightTimer <= 0f)
        {
            explosionLight.enabled = false;
        }
    }
}