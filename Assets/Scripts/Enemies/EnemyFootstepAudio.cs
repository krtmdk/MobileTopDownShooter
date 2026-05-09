using UnityEngine;
using UnityEngine.AI;

// Ётот скрипт отвечает за звуки шагов врага.
// „тобы не было звуковой каши, не каждый враг получает активные шаги.
public class EnemyFootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float volume = 0.3f;

    [Header("Step Timing")]
    [SerializeField] private float stepInterval = 0.6f;
    [SerializeField] private float minSpeedToPlay = 0.2f;

    [Header("Anti Noise Settings")]
    [SerializeField] private float chanceToHaveFootsteps = 0.35f;
    // Ўанс, что конкретный враг вообще будет издавать шаги.
    // 0.35 = примерно 35% врагов будут слышны.

    [SerializeField] private float randomStartDelayMin = 0f;
    // ћинимальна€ случайна€ задержка перед первым шагом.

    [SerializeField] private float randomStartDelayMax = 0.8f;
    // ћаксимальна€ случайна€ задержка перед первым шагом.

    [Header("Pitch Settings")]
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    private float stepTimer;
    private bool footstepsEnabled;

    private void Awake()
    {
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        footstepsEnabled = Random.value <= chanceToHaveFootsteps;
        stepTimer = Random.Range(randomStartDelayMin, randomStartDelayMax);
    }

    private void Update()
    {
        if (!footstepsEnabled)
        {
            return;
        }

        if (navMeshAgent == null)
        {
            return;
        }

        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        Vector3 velocity = navMeshAgent.velocity;
        velocity.y = 0f;

        float speed = velocity.magnitude;

        if (speed < minSpeedToPlay)
        {
            stepTimer = Random.Range(randomStartDelayMin, randomStartDelayMax);
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer > 0f)
        {
            return;
        }

        PlayFootstep();

        float speedMultiplier = Mathf.Clamp(speed / 3.5f, 0.7f, 1.4f);
        stepTimer = stepInterval / speedMultiplier;
    }

    private void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            return;
        }

        AudioClip selectedClip = footstepClips[Random.Range(0, footstepClips.Length)];

        if (selectedClip == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("EnemyFootstepSound");
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