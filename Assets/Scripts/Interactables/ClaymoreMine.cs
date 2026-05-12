using UnityEngine;

public class ClaymoreMine : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private float triggerRadius = 2f;
    // Радиус обнаружения врага

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    // Радиус урона

    [SerializeField] private int explosionDamage = 5;
    // Урон взрыва

    [SerializeField] private float triggerDelay = 0.2f;
    // Задержка перед взрывом после срабатывания

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffectPrefab;
    // Prefab визуального эффекта взрыва

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    // Звук взрыва

    [SerializeField] private float explosionVolume = 1f;
    // Громкость взрыва

    private bool hasExploded;
    // Защита от повторного взрыва

    private bool isTriggered;
    // Мина уже сработала и ждёт задержку

    private float currentTriggerTimer;
    // Таймер задержки перед взрывом

    private void Update()
    {
        if (hasExploded)
        {
            return;
        }

        if (isTriggered)
        {
            UpdateTriggerTimer();
        }
        else
        {
            CheckForEnemies();
        }
    }

    private void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, triggerRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                isTriggered = true;
                currentTriggerTimer = triggerDelay;
                return;
            }
        }
    }

    private void UpdateTriggerTimer()
    {
        currentTriggerTimer -= Time.deltaTime;

        if (currentTriggerTimer <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;

        SpawnExplosionEffect();
        PlayExplosionSound();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            

            EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);
            }

            ExplosiveBarrel barrel = hitCollider.GetComponentInParent<ExplosiveBarrel>();

            if (barrel != null)
            {
                barrel.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    private void SpawnExplosionEffect()
    {
        if (explosionEffectPrefab == null)
        {
            return;
        }

        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    }

    private void PlayExplosionSound()
    {
        if (explosionSound == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("ExplosionSound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = explosionSound;
        source.volume = explosionVolume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, explosionSound.length);
    }
}