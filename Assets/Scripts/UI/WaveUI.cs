using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TextMeshProUGUI waveText;

    private void Update()
    {
        if (waveManager == null || waveText == null)
        {
            return;
        }

        // Если матч уже завершён, скрываем текст волны.
        if (waveManager.IsMatchFinished())
        {
            waveText.text = "";
            return;
        }

        if (waveManager.IsIntermission())
        {
            waveText.text = "Next Wave In: " + Mathf.CeilToInt(waveManager.GetIntermissionTime());
            return;
        }

        if (waveManager.IsWaveClearing())
        {
            waveText.text = "Wave " + waveManager.GetCurrentWave() + "\nClear Remaining Enemies";
            return;
        }

        int current = waveManager.GetKillsThisWave();
        int required = waveManager.GetKillsRequired();

        waveText.text = "Wave " + waveManager.GetCurrentWave() +
                        "\nKills: " + current + " / " + required;
    }
}