using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Barrel Health")]
    [SerializeField] private int maxHealth = 3;
    // Сколько урона выдерживает бочка до взрыва.

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    // Радиус взрыва.

    [SerializeField] private int explosionDamage = 3;
    // Урон по врагам и другим бочкам.

    private int currentHealth;
    // Текущее здоровье бочки.

    private bool hasExploded;
    // Защита от повторного взрыва.

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

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Урон врагам
            EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);
            }

            // Урон другим бочкам
            ExplosiveBarrel otherBarrel = hitCollider.GetComponentInParent<ExplosiveBarrel>();

            // Важно не дамажить саму себя
            if (otherBarrel != null && otherBarrel != this)
            {
                otherBarrel.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}