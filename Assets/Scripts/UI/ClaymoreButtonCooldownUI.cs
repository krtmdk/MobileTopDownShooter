using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClaymoreButtonCooldownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image buttonImage;
    // Картинка кнопки мины.

    [SerializeField] private TextMeshProUGUI cooldownText;
    // Текст, который показывает оставшееся время кулдауна.

    [SerializeField] private PlayerClaymorePlacer claymorePlacer;
    // Ссылка на скрипт клеймора у игрока.

    [Header("Colors")]
    [SerializeField] private Color readyColor = Color.white;
    // Цвет кнопки, когда мина доступна.

    [SerializeField] private Color cooldownColor = Color.gray;
    // Цвет кнопки, когда мина на кулдауне.

    private void Start()
    {
        // Если ссылка не назначена вручную,
        // пробуем найти игрока по тегу и взять его компонент.
        if (claymorePlacer == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                claymorePlacer = playerObject.GetComponent<PlayerClaymorePlacer>();
            }
        }
    }

    private void Update()
    {
        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        if (buttonImage == null || claymorePlacer == null)
        {
            return;
        }

        if (claymorePlacer.IsOnCooldown())
        {
            // Если мина на кулдауне, затемняем кнопку.
            buttonImage.color = cooldownColor;

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.CeilToInt(claymorePlacer.GetCurrentCooldown()).ToString();
            }
        }
        else
        {
            // Если мина готова, возвращаем обычный цвет.
            buttonImage.color = readyColor;

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(false);
            }
        }
    }
}