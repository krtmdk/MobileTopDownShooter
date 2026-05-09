using UnityEngine;

// Этот скрипт отвечает за голосовые звуки врага.
// При появлении враг может проиграть alert-звук.
// Затем периодически проигрывает idle-звуки.
public class EnemyVoiceAudio : MonoBehaviour
{
    [Header("Alert Sounds")]
    [SerializeField] private AudioClip[] alertClips;
    // Звуки обнаружения / появления врага.

    [SerializeField] private float alertVolume = 0.45f;
    // Громкость alert-звука.

    [SerializeField] private bool playAlertOnStart = true;
    // Играть ли alert при появлении врага.

    [Header("Idle Sounds")]
    [SerializeField] private AudioClip[] idleClips;
    // Звуки дыхания, стона, бормотания.

    [SerializeField] private float idleVolume = 0.35f;
    // Громкость idle-звуков.

    [SerializeField] private float minIdleDelay = 4f;
    // Минимальная пауза между idle-звуками.

    [SerializeField] private float maxIdleDelay = 8f;
    // Максимальная пауза между idle-звуками.

    [Header("Pitch Settings")]
    [SerializeField] private float minPitch = 0.95f;
    // Минимальный pitch.

    [SerializeField] private float maxPitch = 1.05f;
    // Максимальный pitch.

    private float idleTimer;
    // Таймер до следующего idle-звука.

    private void Start()
    {
        if (playAlertOnStart)
        {
            PlayRandomSound(alertClips, alertVolume);
        }

        ResetIdleTimer();
    }

    private void Update()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer > 0f)
        {
            return;
        }

        PlayRandomSound(idleClips, idleVolume);
        ResetIdleTimer();
    }

    private void ResetIdleTimer()
    {
        idleTimer = Random.Range(minIdleDelay, maxIdleDelay);
    }

    private void PlayRandomSound(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        AudioClip selectedClip = clips[randomIndex];

        if (selectedClip == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("EnemyVoiceSound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = selectedClip;
        source.volume = volume;
        source.pitch = Random.Range(minPitch, maxPitch);
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, selectedClip.length);
    }
}