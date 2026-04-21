using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    // Максимальное здоровье игрока.

    [Header("UI References")]
    [SerializeField] private GameOverUI gameOverUI;
    // Ссылка на UI-скрипт, который показывает экран поражения.

    private int currentHealth;
    // Текущее здоровье игрока.

    private bool isDead;
    // Флаг, который показывает, умер игрок или нет.

    private void Awake()
    {
        // При старте игрок получает полное здоровье.
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Если игрок уже мёртв, не принимаем новый урон.
        if (isDead)
        {
            return;
        }

        // Защита от некорректного значения урона.
        if (damage <= 0)
        {
            return;
        }

        // Уменьшаем здоровье.
        currentHealth -= damage;

        // Не даём здоровью уйти ниже нуля.
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("Player took damage. Current health: " + currentHealth);

        // Если здоровье закончилось, игрок умирает.
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

        Debug.Log("Player is dead.");

        // Сначала показываем экран поражения.
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }

        // Потом выключаем игрока.
        gameObject.SetActive(false);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void Heal(int amount)
    {
        // Если игрок уже мёртв, не лечим
        if (isDead)
        {
            return;
        }

        // Защита от некорректного значения
        if (amount <= 0)
        {
            return;
        }

        // Добавляем здоровье
        currentHealth += amount;

        // Ограничиваем максимумом
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Player healed. Current health: " + currentHealth);
    }
}