using UnityEngine;

// Этот скрипт удаляет эффект через короткое время
public class HitEffectAutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}