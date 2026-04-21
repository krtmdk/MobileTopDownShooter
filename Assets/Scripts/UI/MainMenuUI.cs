using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string loadingSceneName = "LoadingScene";
    // Имя сцены загрузки, куда переходим после нажатия Play.

    public void OnPlayPressed()
    {
        // Загружаем сцену загрузки.
        SceneManager.LoadScene(loadingSceneName);
    }

    public void OnExitPressed()
    {
        // Пытаемся выйти из игры.
        // В редакторе Unity это не закроет сам редактор, и это нормально.
        Application.Quit();

        Debug.Log("Exit Game");
    }
}