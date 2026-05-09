using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicPlayer : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;

    [SerializeField] private float volume = 0.45f;

    [SerializeField] private float fadeInTime = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = menuMusic;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        if (menuMusic != null)
        {
            audioSource.Play();
            StartCoroutine(FadeVolume(0f, volume, fadeInTime));
        }
    }

    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            audioSource.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }

        audioSource.volume = to;
    }
}