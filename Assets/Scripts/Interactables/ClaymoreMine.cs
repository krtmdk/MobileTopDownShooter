using UnityEngine;

public class ClaymoreMine : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private float triggerRadius = 2f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 5;
    [SerializeField] private float explosionAngle = 90f;
    [SerializeField] private float triggerDelay = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float explosionVolume = 1f;

    private bool hasExploded;
    private bool isTriggered;
    private float currentTriggerTimer;

    private void Update()
    {
        if (hasExploded)
        {
            return;
        }

        if (isTriggered)
        {
            currentTriggerTimer -= Time.deltaTime;

            if (currentTriggerTimer <= 0f)
            {
                Explode();
            }
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
            Vector3 directionToTarget = hitCollider.transform.position - transform.position;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude <= 0.001f)
            {
                continue;
            }

            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget > explosionAngle * 0.5f)
            {
                continue;
            }

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