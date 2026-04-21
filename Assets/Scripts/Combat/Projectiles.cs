using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 12f;
    // Скорость полёта снаряда.

    [SerializeField] private float lifetime = 1f;
    // Время жизни снаряда, если он никуда не попал.

    [SerializeField] private int damage = 1;
    // Урон, который наносит снаряд.

    private Rigidbody rb;
    // Rigidbody снаряда.

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Урон врагу
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        // Урон бочке
        ExplosiveBarrel explosiveBarrel = collision.gameObject.GetComponentInParent<ExplosiveBarrel>();

        if (explosiveBarrel != null)
        {
            explosiveBarrel.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}