using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Barrel Health")]
    [SerializeField] private int maxHealth = 3;
    // Сколько урона выдерживает бочка

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    // Радиус урона

    [SerializeField] private int explosionDamage = 3;
    // Урон взрыва

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffectPrefab;
    // Prefab визуального эффекта взрыва

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    // Звук взрыва

    [SerializeField] private float explosionVolume = 1f;
    // Громкость взрыва

    private int currentHealth;
    // Текущее здоровье бочки

    private bool hasExploded;
    // Защита от повторного взрыва

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (hasExploded)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
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

            ExplosiveBarrel otherBarrel = hitCollider.GetComponentInParent<ExplosiveBarrel>();

            if (otherBarrel != null && otherBarrel != this)
            {
                otherBarrel.TakeDamage(explosionDamage);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}