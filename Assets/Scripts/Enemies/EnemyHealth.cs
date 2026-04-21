using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    // Максимальное здоровье врага.

    private int currentHealth;
    // Текущее здоровье врага.

    private bool isDead;
    // Флаг, чтобы не обрабатывать смерть несколько раз.

    private KillCounter killCounter;
    // Ссылка на счётчик убийств.

    private void Awake()
    {
        currentHealth = maxHealth;

        killCounter = FindObjectOfType<KillCounter>();
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
        if (isDead)
        {
            return;
        }

        isDead = true;

        // Если счётчик найден, увеличиваем число убийств.
        if (killCounter != null)
        {
            killCounter.RegisterKill();
        }

        // Пока просто удаляем врага со сцены.
        Destroy(gameObject);
    }
}