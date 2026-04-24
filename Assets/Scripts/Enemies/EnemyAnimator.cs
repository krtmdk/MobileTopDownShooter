using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody enemyRigidbody;
    // Rigidbody врага. По нему считаем скорость движения.

    [SerializeField] private Animator enemyAnimator;
    // Animator модели врага.

    [SerializeField] private EnemyRiotCharge riotCharge;
    // Скрипт рывка громилы. У обычных врагов может отсутствовать.

    private void Awake()
    {
        if (enemyRigidbody == null)
        {
            enemyRigidbody = GetComponent<Rigidbody>();
        }

        if (riotCharge == null)
        {
            riotCharge = GetComponent<EnemyRiotCharge>();
        }
    }

    private void Update()
    {
        UpdateMovementAnimation();
        UpdateRiotAnimation();
    }

    private void UpdateMovementAnimation()
    {
        if (enemyRigidbody == null || enemyAnimator == null)
        {
            return;
        }

        Vector3 velocity = enemyRigidbody.velocity;
        velocity.y = 0f;

        float speed = velocity.magnitude;

        enemyAnimator.SetFloat("Speed", speed);
    }

    private void UpdateRiotAnimation()
    {
        if (enemyAnimator == null)
        {
            return;
        }

        if (riotCharge == null)
        {
            return;
        }

        enemyAnimator.SetBool("IsCharging", riotCharge.IsCharging());
    }

    public void PlayAttack()
    {
        if (enemyAnimator == null)
        {
            return;
        }

        enemyAnimator.ResetTrigger("Attack");
        enemyAnimator.SetTrigger("Attack");
    }

    public void PlayDeath()
    {
        if (enemyAnimator == null)
        {
            return;
        }

        enemyAnimator.ResetTrigger("Death");
        enemyAnimator.SetTrigger("Death");
    }
}