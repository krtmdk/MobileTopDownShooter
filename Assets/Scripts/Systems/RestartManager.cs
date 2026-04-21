using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    // Этот метод будет вызываться кнопкой Restart
    public void RestartGame()
    {
        // Получаем текущую активную сцену
        Scene currentScene = SceneManager.GetActiveScene();

        // Перезагружаем её по имени
        SceneManager.LoadScene(currentScene.name);
    }
}