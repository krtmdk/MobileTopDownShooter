using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [Header("Scene To Load")]
    [SerializeField] private string gameSceneName = "Prototype_Game";

    [Header("Loading Settings")]
    [SerializeField] private float minimumLoadTime = 2f;

    [Header("Backgrounds")]
    [SerializeField] private Image backgroundImage;
    // Ссылка на UI Image (фон)

    [SerializeField] private Sprite[] backgrounds;
    // Массив картинок для загрузки

    private void Start()
    {
        SetRandomBackground();
        StartCoroutine(LoadGameSceneRoutine());
    }

    private void SetRandomBackground()
    {
        // Проверка
        if (backgroundImage == null || backgrounds == null || backgrounds.Length == 0)
        {
            Debug.LogWarning("Backgrounds are not assigned!");
            return;
        }

        // Берём случайный индекс
        int randomIndex = Random.Range(0, backgrounds.Length);

        // Меняем картинку
        backgroundImage.sprite = backgrounds[randomIndex];
    }

    private IEnumerator LoadGameSceneRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);

        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            if (operation.progress >= 0.9f && timer >= minimumLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}