using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Death Settings")]
    [SerializeField] private float deathAnimationDuration = 1.2f;

    [Header("Animation References")]
    [SerializeField] private EnemyAnimator enemyAnimator;

    [Header("Audio")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private float hitVolume = 0.7f;
    [SerializeField] private float deathVolume = 0.8f;

    private int currentHealth;
    private bool isDead;

    private KillCounter killCounter;
    private Rigidbody rb;
    private Collider enemyCollider;
    private EnemyChase enemyChase;
    private EnemyContactDamage enemyContactDamage;
    private EnemyRiotCharge enemyRiotCharge;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        currentHealth = maxHealth;

        killCounter = FindObjectOfType<KillCounter>();

        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        enemyChase = GetComponent<EnemyChase>();
        enemyContactDamage = GetComponent<EnemyContactDamage>();
        enemyRiotCharge = GetComponent<EnemyRiotCharge>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<EnemyAnimator>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }


        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        PlayRandomSound(hitSounds, hitVolume);
    }
    

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        PlayRandomSound(deathSounds, deathVolume);

        if (killCounter != null)
        {
            killCounter.RegisterKill();
        }

        if (enemyChase != null)
        {
            enemyChase.enabled = false;
        }

        if (enemyContactDamage != null)
        {
            enemyContactDamage.enabled = false;
        }

        if (enemyRiotCharge != null)
        {
            enemyRiotCharge.enabled = false;
        }

        if (navMeshAgent != null)
        {
            if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }

            navMeshAgent.enabled = false;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.PlayDeath();
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        Destroy(gameObject);
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

        GameObject soundObject = new GameObject("EnemySound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = selectedClip;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, selectedClip.length);
    }
}