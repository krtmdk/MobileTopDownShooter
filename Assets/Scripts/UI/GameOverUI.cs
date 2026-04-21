using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;
    // Панель Game Over.

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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel == null)
        {
            Debug.LogWarning("GameOverPanel is not assigned.");
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

        gameOverPanel.SetActive(true);
    }
}