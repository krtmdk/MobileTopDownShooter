using TMPro;
using UnityEngine;

// Этот скрипт показывает количество гранат и клейморов у игрока.
public class EquipmentCountUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerGrenadeThrower grenadeThrower;
    // Ссылка на скрипт гранат у игрока.

    [SerializeField] private PlayerClaymorePlacer claymorePlacer;
    // Ссылка на скрипт клейморов у игрока.

    [SerializeField] private TextMeshProUGUI grenadeCountText;
    // Текст количества гранат.

    [SerializeField] private TextMeshProUGUI claymoreCountText;
    // Текст количества клейморов.

    private void Start()
    {
        if (grenadeThrower == null || claymorePlacer == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                grenadeThrower = playerObject.GetComponent<PlayerGrenadeThrower>();
                claymorePlacer = playerObject.GetComponent<PlayerClaymorePlacer>();
            }
        }

        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (grenadeThrower != null && grenadeCountText != null)
        {
            grenadeCountText.text = " " + grenadeThrower.GetCurrentGrenades();
        }

        if (claymorePlacer != null && claymoreCountText != null)
        {
            claymoreCountText.text = " " + claymorePlacer.GetCurrentClaymores();
        }
    }
}