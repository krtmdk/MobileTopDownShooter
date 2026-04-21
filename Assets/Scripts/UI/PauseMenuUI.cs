using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pausePanel;
    // Панель паузы.

    [SerializeField] private GameObject gameOverPanel;
    // Панель проигрыша.

    [SerializeField] private GameObject victoryPanel;
    // Панель победы.

    [SerializeField] private PlayerInputReader inputReader;
    // Источник ввода.

    [SerializeField] private GameplayUIVisibility gameplayUIVisibility;
    // Скрипт, который управляет видимостью обычного HUD и мобильного UI.

    private bool isPaused;

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
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(true);
        }

        if (gameplayUIVisibility != null)
        {
            gameplayUIVisibility.ShowGameplayUI();
        }
    }

    private void Update()
    {
        HandlePauseInput();
    }

    private void HandlePauseInput()
    {
        if (inputReader == null)
        {
            return;
        }

        if (!inputReader.PausePressed)
        {
            return;
        }

        if (pausePanel == null)
        {
            return;
        }

        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            return;
        }

        if (victoryPanel != null && victoryPanel.activeSelf)
        {
            return;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pausePanel == null)
        {
            return;
        }

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(false);
        }

        if (gameplayUIVisibility != null)
        {
            gameplayUIVisibility.HideGameplayUI();
        }
    }

    public void ResumeGame()
    {
        if (pausePanel == null)
        {
            return;
        }

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(true);
        }

        if (gameplayUIVisibility != null)
        {
            gameplayUIVisibility.ShowGameplayUI();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(true);
        }

        RestartManager restartManager = GetComponent<RestartManager>();

        if (restartManager != null)
        {
            restartManager.RestartGame();
        }
        else
        {
            Debug.LogWarning("RestartManager was not found on the same object.");
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(true);
        }

        Debug.Log("QuitGame called.");

        Application.Quit();
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}