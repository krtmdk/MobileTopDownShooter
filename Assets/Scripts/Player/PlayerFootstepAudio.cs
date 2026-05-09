using UnityEngine;

// Этот скрипт отвечает за звуки шагов игрока.
// Он проверяет скорость Rigidbody и проигрывает случайный звук шага через заданный интервал.
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody playerRigidbody;
    // Rigidbody игрока. По нему определяем, движется ли игрок.

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip[] footstepClips;
    // Массив звуков шагов игрока.

    [SerializeField] private float volume = 0.45f;
    // Громкость шагов игрока.

    [Header("Step Timing")]
    [SerializeField] private float stepInterval = 0.42f;
    // Интервал между шагами. Чем меньше значение, тем чаще шаги.

    [SerializeField] private float minSpeedToPlay = 0.2f;
    // Минимальная скорость, при которой шаги начинают проигрываться.

    private float stepTimer;
    // Таймер до следующего шага.

    private void Awake()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (playerRigidbody == null)
        {
            return;
        }

        Vector3 velocity = playerRigidbody.velocity;
        velocity.y = 0f;

        float speed = velocity.magnitude;

        if (speed < minSpeedToPlay)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer > 0f)
        {
            return;
        }

        PlayFootstep();

        float speedMultiplier = Mathf.Clamp(speed / 5f, 0.7f, 1.3f);
        stepTimer = stepInterval / speedMultiplier;
    }

    private void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, footstepClips.Length);
        AudioClip selectedClip = footstepClips[randomIndex];

        if (selectedClip == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("PlayerFootstepSound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = selectedClip;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, selectedClip.length);
    }
}