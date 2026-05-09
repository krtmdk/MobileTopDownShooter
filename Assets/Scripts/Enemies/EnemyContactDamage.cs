using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Animation References")]
    [SerializeField] private EnemyAnimator enemyAnimator;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float attackVolume = 0.7f;

    private float currentCooldown;

    private void Awake()
    {
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<EnemyAnimator>();
        }
    }

    private void Update()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown < 0f)
            {
                currentCooldown = 0f;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (currentCooldown > 0f)
        {
            return;
        }

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.PlayAttack();
        }

        PlayAttackSound();

        playerHealth.TakeDamage(damage);

        currentCooldown = damageCooldown;
    }

    private void PlayAttackSound()
    {
        if (attackSound == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("EnemyAttackSound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = attackSound;
        source.volume = attackVolume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, attackSound.length);
    }
}