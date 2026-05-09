using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody enemyRigidbody;
    // Rigidbody врага. Используется как запасной вариант.

    [SerializeField] private NavMeshAgent navMeshAgent;
    // NavMeshAgent врага. Если есть, скорость берём из него.

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

        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
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
        if (enemyAnimator == null)
        {
            return;
        }

        float speed = 0f;

        if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            Vector3 velocity = navMeshAgent.velocity;
            velocity.y = 0f;
            speed = velocity.magnitude;
        }
        else if (enemyRigidbody != null)
        {
            Vector3 velocity = enemyRigidbody.velocity;
            velocity.y = 0f;
            speed = velocity.magnitude;
        }

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