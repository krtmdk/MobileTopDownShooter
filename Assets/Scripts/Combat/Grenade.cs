using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float fuseTime = 1.5f;
    // Через сколько секунд граната взрывается

    [SerializeField] private float explosionRadius = 3f;
    // Радиус урона

    [SerializeField] private int explosionDamage = 4;
    // Урон взрыва

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

    private void Start()
    {
        Invoke(nameof(Explode), fuseTime);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}