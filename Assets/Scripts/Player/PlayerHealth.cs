using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("UI References")]
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Animation References")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerInputReader inputReader;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    // AudioSource игрока.

    [SerializeField] private AudioClip[] hitSounds;
    // Звуки получения урона. Можно добавить player_hit1 и player_hit2.

    [SerializeField] private AudioClip deathSound;
    // Звук смерти игрока.

    [Header("Death Settings")]
    [SerializeField] private float deathAnimationDuration = 1.2f;

    private int currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("Player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        PlayRandomHitSound();
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("Player is dead.");

        PlaySound(deathSound);

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(false);
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Death");
            playerAnimator.SetTrigger("Death");
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }

        gameObject.SetActive(false);
    }

    private void PlayRandomHitSound()
    {
        if (hitSounds == null || hitSounds.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, hitSounds.Length);
        PlaySound(hitSounds[randomIndex]);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null)
        {
            return;
        }

        if (clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void Heal(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Player healed. Current health: " + currentHealth);
    }
}