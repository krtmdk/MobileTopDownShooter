using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    // Ссылка на компонент здоровья игрока.

    [SerializeField] private TextMeshProUGUI healthText;
    // Ссылка на текстовый элемент интерфейса.

    private void Start()
    {
        // Обновляем текст сразу после старта сцены.
        UpdateHealthText();
    }

    private void Update()
    {
        // Пока для простоты обновляем текст каждый кадр.
        // Позже можно заменить это на обновление только по событию.
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        // Если нужные ссылки не назначены, ничего не делаем.
        if (playerHealth == null || healthText == null)
        {
            return;
        }

        // Получаем значения здоровья игрока.
        int currentHealth = playerHealth.GetCurrentHealth();
        int maxHealth = playerHealth.GetMaxHealth();

        // Обновляем текст на экране.
        healthText.text = "HP: " + currentHealth + " / " + maxHealth;
    }
}