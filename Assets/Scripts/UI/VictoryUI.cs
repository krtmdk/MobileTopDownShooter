using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject victoryPanel;
    // Панель победы.

    [SerializeField] private PlayerInputReader inputReader;
    // Источник ввода.

    [SerializeField] private GameplayUIVisibility gameplayUIVisibility;
    // Управление видимостью обычного HUD и мобильного UI.

    private void Awake()
    {
        if (inputReader == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                inputReader = playerObject.GetComponent<PlayerInputReader>();
            }
        }

        if (gameplayUIVisibility == null)
        {
            gameplayUIVisibility = FindObjectOfType<GameplayUIVisibility>();
        }
    }

    private void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    public void ShowVictory()
    {
        if (victoryPanel == null)
        {
            Debug.LogWarning("VictoryPanel is not assigned.");
            return;
        }

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(false);
        }

        if (gameplayUIVisibility != null)
        {
            gameplayUIVisibility.HideGameplayUI();
        }

        victoryPanel.SetActive(true);
    }
}