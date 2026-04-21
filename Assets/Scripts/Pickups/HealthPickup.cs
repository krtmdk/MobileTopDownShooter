using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int healAmount = 3;
    // Сколько здоровья восстанавливает аптечка.
    private void Update()
    {
        transform.Rotate(0f, 100f * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, есть ли у объекта компонент PlayerHealth
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        // Лечим игрока
        playerHealth.Heal(healAmount);

        // Уничтожаем аптечку после использования
        Destroy(gameObject);
    }
}