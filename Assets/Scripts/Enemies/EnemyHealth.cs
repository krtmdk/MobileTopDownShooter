using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    // Максимальное здоровье врага.

    [Header("Death Settings")]
    [SerializeField] private float deathAnimationDuration = 1.2f;
    // Сколько секунд ждать перед удалением врага после запуска анимации смерти.

    [Header("Animation References")]
    [SerializeField] private EnemyAnimator enemyAnimator;
    // Скрипт, который запускает анимации врага.

    private int currentHealth;
    // Текущее здоровье врага.

    private bool isDead;
    // Флаг, чтобы не обрабатывать смерть несколько раз.

    private KillCounter killCounter;
    // Ссылка на счётчик убийств.

    private Rigidbody rb;
    // Rigidbody врага.

    private Collider enemyCollider;
    // Основной коллайдер врага.

    private EnemyChase enemyChase;
    // Скрипт движения врага.

    private EnemyContactDamage enemyContactDamage;
    // Скрипт контактного урона врага.

    private EnemyRiotCharge enemyRiotCharge;
    // Скрипт рывка тяжёлого врага. У обычного врага может отсутствовать.

    private void Awake()
    {
        currentHealth = maxHealth;

        killCounter = FindObjectOfType<KillCounter>();

        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        enemyChase = GetComponent<EnemyChase>();
        enemyContactDamage = GetComponent<EnemyContactDamage>();
        enemyRiotCharge = GetComponent<EnemyRiotCharge>();

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<EnemyAnimator>();
        }
    }

    public void TakeDamage(int damage)
    {
        // Если враг уже мёртв, новый урон не обрабатываем.
        if (isDead)
        {
            return;
        }

        // Если урон некорректный, выходим.
        if (damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        // Не даём здоровью уйти ниже нуля.
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Защита от повторной смерти.
        if (isDead)
        {
            return;
        }

        isDead = true;

        // Засчитываем убийство сразу, чтобы счётчик не ждал удаления объекта.
        if (killCounter != null)
        {
            killCounter.RegisterKill();
        }

        // Отключаем движение, чтобы мёртвый враг не продолжал идти.
        if (enemyChase != null)
        {
            enemyChase.enabled = false;
        }

        // Отключаем контактный урон, чтобы мёртвый враг не бил игрока.
        if (enemyContactDamage != null)
        {
            enemyContactDamage.enabled = false;
        }

        // Если это тяжёлый враг с рывком, отключаем рывок.
        if (enemyRiotCharge != null)
        {
            enemyRiotCharge.enabled = false;
        }

        // Отключаем коллайдер, чтобы мёртвый враг не толкал игрока и других врагов.
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // Останавливаем физику врага.
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Запускаем анимацию смерти.
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayDeath();
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        Destroy(gameObject);
    }
}