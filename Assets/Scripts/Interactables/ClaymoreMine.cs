using UnityEngine;

public class ClaymoreMine : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private float triggerRadius = 2f;
    // Радиус, в котором клеймор замечает врага и срабатывает.

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    // Радиус, в котором ищем цели для урона.

    [SerializeField] private int explosionDamage = 5;
    // Урон по врагам и бочкам.

    [SerializeField] private float explosionAngle = 90f;
    // Угол сектора поражения.
    // Например 90 = 45 градусов влево и 45 вправо от направления мины.

    [SerializeField] private float triggerDelay = 0.2f;
    // Небольшая задержка перед взрывом после обнаружения врага.

    private bool hasExploded;
    // Защита от повторного взрыва.

    private bool isTriggered;
    // Клеймор уже сработала, но ещё ждёт triggerDelay.

    private float currentTriggerTimer;
    // Таймер задержки перед взрывом.

    private void Update()
    {
        if (hasExploded)
        {
            return;
        }

        if (isTriggered)
        {
            UpdateTriggerDelay();
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

    private void UpdateTriggerDelay()
    {
        currentTriggerTimer -= Time.deltaTime;

        if (currentTriggerTimer > 0f)
        {
            return;
        }

        Explode();
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
            // Считаем направление от мины к объекту.
            // Нужно, чтобы урон проходил только в секторе перед клеймором.
            Vector3 directionToTarget = hitCollider.transform.position - transform.position;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude <= 0.001f)
            {
                continue;
            }

            // Проверяем угол между направлением мины и направлением на цель.
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            // Если объект не попадает в сектор поражения, пропускаем его.
            if (angleToTarget > explosionAngle * 0.5f)
            {
                continue;
            }

            // Урон врагам
            EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);
            }

            // Урон бочкам
            ExplosiveBarrel barrel = hitCollider.GetComponentInParent<ExplosiveBarrel>();

            if (barrel != null)
            {
                barrel.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Жёлтый круг — радиус срабатывания.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        // Красный круг — радиус поиска целей для взрыва.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        // Синяя линия — направление вперёд для сектора поражения.
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * explosionRadius
        );
    }
}