using UnityEngine;

// Этот скрипт управляет курсором в ПК-билде.
// В геймплее курсор скрыт.
// В паузе, победе и поражении курсор снова виден.
public class PCCursorSettings : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PauseMenuUI pauseMenuUI;
    // Ссылка на меню паузы

    [SerializeField] private GameObject gameOverPanel;
    // Панель поражения

    [SerializeField] private GameObject victoryPanel;
    // Панель победы

    [Header("Settings")]
    [SerializeField] private bool hideCursorDuringGameplay = true;
    // Нужно ли скрывать курсор во время игры

    private void Awake()
    {
        if (pauseMenuUI == null)
        {
            pauseMenuUI = FindObjectOfType<PauseMenuUI>();
        }
    }

    private void Update()
    {
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        bool shouldShowCursor = false;

        if (pauseMenuUI != null && pauseMenuUI.IsPaused())
        {
            shouldShowCursor = true;
        }

        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            shouldShowCursor = true;
        }

        if (victoryPanel != null && victoryPanel.activeSelf)
        {
            shouldShowCursor = true;
        }

        if (!hideCursorDuringGameplay)
        {
            shouldShowCursor = true;
        }

        Cursor.visible = shouldShowCursor;
        Cursor.lockState = CursorLockMode.None;
    }
}