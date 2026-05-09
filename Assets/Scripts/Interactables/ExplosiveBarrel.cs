using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Barrel Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 3;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float explosionVolume = 1f;

    private int currentHealth;
    private bool hasExploded;

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