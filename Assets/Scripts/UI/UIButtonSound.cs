using UnityEngine;
using UnityEngine.EventSystems;

// Этот скрипт проигрывает звук клика по UI-кнопке.
// Его можно повесить на кнопки в главном меню, паузе, победе, поражении.
public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    [Header("Audio")]
    [SerializeField] private AudioClip clickSound;
    // Звук клика кнопки.

    [SerializeField] private float volume = 0.8f;
    // Громкость клика.

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        if (clickSound == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("UIButtonClickSound");

        // Важно: объект не уничтожится при смене сцены.
        // Поэтому звук успеет доиграть даже если кнопка Play сразу грузит LoadingScene.
        DontDestroyOnLoad(soundObject);

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = clickSound;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, clickSound.length);
    }
}