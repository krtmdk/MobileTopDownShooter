using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float fuseTime = 1.5f;
    // Через сколько секунд граната взрывается после броска.

    [SerializeField] private float explosionRadius = 3f;
    // Радиус взрыва гранаты.

    [SerializeField] private int explosionDamage = 4;
    // Урон гранаты по врагам.

    private bool hasExploded;
    // Защита от повторного взрыва.

    private void Start()
    {
        // Запускаем таймер взрыва сразу после появления гранаты.
        Invoke(nameof(Explode), fuseTime);
    }

    private void Explode()
    {
        // Если граната уже взорвалась, повторно ничего не делаем.
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;

        // Находим все коллайдеры в радиусе взрыва.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Ищем здоровье врага на объекте или у родителя.
            EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);
            }

            // Урон бочкам (НОВОЕ)
            ExplosiveBarrel barrel = hitCollider.GetComponentInParent<ExplosiveBarrel>();

            if (barrel != null)
            {
                barrel.TakeDamage(explosionDamage);
            }
        }

        // После взрыва удаляем объект гранаты.
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Показываем радиус взрыва в редакторе.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}