using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeButtonCooldownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image buttonImage;
    // Картинка кнопки гранаты.

    [SerializeField] private TextMeshProUGUI cooldownText;
    // Текст, который будет показывать оставшееся время кулдауна.

    [SerializeField] private PlayerGrenadeThrower grenadeThrower;
    // Ссылка на скрипт гранаты у игрока.

    [Header("Colors")]
    [SerializeField] private Color readyColor = Color.white;
    // Цвет кнопки, когда граната доступна.

    [SerializeField] private Color cooldownColor = Color.gray;
    // Цвет кнопки, когда граната на кулдауне.

    private void Start()
    {
        // Если ссылка не назначена вручную,
        // пробуем найти игрока по тегу и взять его компонент.
        if (grenadeThrower == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                grenadeThrower = playerObject.GetComponent<PlayerGrenadeThrower>();
            }
        }
    }

    private void Update()
    {
        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        if (buttonImage == null || grenadeThrower == null)
        {
            return;
        }

        if (grenadeThrower.IsOnCooldown())
        {
            // Если граната на кулдауне, затемняем кнопку.
            buttonImage.color = cooldownColor;

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.CeilToInt(grenadeThrower.GetCurrentCooldown()).ToString();
            }
        }
        else
        {
            // Если граната готова, возвращаем обычный цвет.
            buttonImage.color = readyColor;

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(false);
            }
        }
    }
}