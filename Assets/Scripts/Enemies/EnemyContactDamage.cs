using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    // Урон за один контактный удар.

    [SerializeField] private float damageCooldown = 1f;
    // Время между ударами.

    [Header("Animation References")]
    [SerializeField] private EnemyAnimator enemyAnimator;
    // Скрипт, который запускает анимации врага.

    private float currentCooldown;
    // Остаток времени до следующего удара.

    private void Awake()
    {
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<EnemyAnimator>();
        }
    }

    private void Update()
    {
        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown < 0f)
            {
                currentCooldown = 0f;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Если враг ещё не может ударить снова, выходим.
        if (currentCooldown > 0f)
        {
            return;
        }

        // Проверяем здоровье игрока на объекте столкновения.
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        // Если столкнулись не с игроком, ничего не делаем.
        if (playerHealth == null)
        {
            return;
        }

        // Запускаем анимацию атаки.
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayAttack();
        }

        // Наносим игроку урон.
        playerHealth.TakeDamage(damage);

        // Включаем задержку до следующего удара.
        currentCooldown = damageCooldown;
    }
}