using TMPro;
using UnityEngine;

public class KillCounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private KillCounter killCounter;
    // Ссылка на объект, который хранит число убийств.

    [SerializeField] private TextMeshProUGUI killCountText;
    // Ссылка на текст UI, который показывает число убийств.

    private void Start()
    {
        UpdateKillText();
    }

    private void Update()
    {
        // Пока для простоты обновляем каждый кадр.
        UpdateKillText();
    }

    private void UpdateKillText()
    {
        if (killCounter == null || killCountText == null)
        {
            return;
        }

        killCountText.text = "Kills: " + killCounter.GetKillCount();
    }
}