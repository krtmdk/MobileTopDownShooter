using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;

    [Header("UI References")]
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Animation References")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerInputReader inputReader;

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
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("Player is dead.");

        if (inputReader != null)
        {
            inputReader.SetInputEnabled(false);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Сначала полностью останавливаем Rigidbody.
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Только потом переводим его в kinematic.
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