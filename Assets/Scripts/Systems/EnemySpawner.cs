using System.Collections.Generic;
using UnityEngine;

// Этот скрипт отвечает за спавн врагов.
// Теперь враги разделены по типам:
// обычные, быстрые и громилы.
// WaveManager будет говорить этому спавнеру,
// какие типы врагов разрешены на текущей волне.
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject normalEnemyPrefab;
    // Префаб обычного врага.

    [SerializeField] private GameObject fastEnemyPrefab;
    // Префаб быстрого врага.

    [SerializeField] private GameObject heavyEnemyPrefab;
    // Префаб громилы.

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    // Точки появления врагов.

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    // Интервал между попытками спавна.

    [SerializeField] private int maxAliveEnemies = 8;
    // Максимум живых врагов одновременно.

    [Header("Dangerous Enemy Limits")]
    [SerializeField] private int maxAliveFastEnemies = 2;
    // Максимум быстрых врагов одновременно.

    [SerializeField] private int maxAliveHeavyEnemies = 1;
    // Максимум громил одновременно.

    [Header("Wave Type Flags")]
    [SerializeField] private bool allowFastEnemies = false;
    // Можно ли сейчас спавнить быстрых врагов.

    [SerializeField] private bool allowHeavyEnemies = false;
    // Можно ли сейчас спавнить громил.

    [Header("Spawn Weights")]
    [SerializeField] private int normalEnemyWeight = 100;
    // Базовый шанс выбора обычного врага.

    [SerializeField] private int fastEnemyWeight = 35;
    // Базовый шанс выбора быстрого врага.

    [SerializeField] private int heavyEnemyWeight = 15;
    // Базовый шанс выбора громилы.

    private float spawnTimer;
    // Таймер до следующего спавна.

    private bool isSpawning = true;
    // Разрешён ли сейчас спавн.

    private void Start()
    {
        // Чтобы первый спавн мог пройти почти сразу.
        spawnTimer = 0f;
    }

    private void Update()
    {
        UpdateSpawnTimer();
    }

    // Этот метод обновляет таймер спавна и пытается заспавнить врага.
    private void UpdateSpawnTimer()
    {
        if (!isSpawning)
        {
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            return;
        }

        if (GetAliveEnemyCount() >= maxAliveEnemies)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0f)
        {
            return;
        }

        SpawnEnemy();

        spawnTimer = spawnInterval;
    }

    // Этот метод выбирает тип врага и создаёт его на случайной точке.
    private void SpawnEnemy()
    {
        GameObject selectedEnemyPrefab = GetEnemyPrefabForCurrentWave();

        if (selectedEnemyPrefab == null)
        {
            return;
        }

        int randomSpawnIndex = Random.Range(0, spawnPoints.Count);
        Transform selectedSpawnPoint = spawnPoints[randomSpawnIndex];

        if (selectedSpawnPoint == null)
        {
            return;
        }

        Instantiate(
            selectedEnemyPrefab,
            selectedSpawnPoint.position,
            selectedSpawnPoint.rotation
        );
    }

    // Этот метод выбирает, какого врага можно заспавнить на текущей волне.
    private GameObject GetEnemyPrefabForCurrentWave()
    {
        List<GameObject> availablePrefabs = new List<GameObject>();
        List<int> availableWeights = new List<int>();

        // Обычные враги доступны всегда.
        if (normalEnemyPrefab != null)
        {
            availablePrefabs.Add(normalEnemyPrefab);
            availableWeights.Add(Mathf.Max(1, normalEnemyWeight));
        }

        // Быстрых врагов добавляем только если они разрешены
        // и не превышен лимит одновременно живых быстрых врагов.
        if (allowFastEnemies && fastEnemyPrefab != null)
        {
            if (GetAliveFastEnemyCount() < maxAliveFastEnemies)
            {
                availablePrefabs.Add(fastEnemyPrefab);
                availableWeights.Add(Mathf.Max(1, fastEnemyWeight));
            }
        }

        // Громил добавляем только если они разрешены
        // и не превышен лимит одновременно живых громил.
        if (allowHeavyEnemies && heavyEnemyPrefab != null)
        {
            if (GetAliveHeavyEnemyCount() < maxAliveHeavyEnemies)
            {
                availablePrefabs.Add(heavyEnemyPrefab);
                availableWeights.Add(Mathf.Max(1, heavyEnemyWeight));
            }
        }

        if (availablePrefabs.Count == 0)
        {
            return null;
        }

        return GetWeightedRandomPrefab(availablePrefabs, availableWeights);
    }

    // Этот метод выбирает случайный префаб по весам.
    private GameObject GetWeightedRandomPrefab(List<GameObject> prefabs, List<int> weights)
    {
        int totalWeight = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            totalWeight += weights[i];
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        for (int i = 0; i < prefabs.Count; i++)
        {
            currentWeightSum += weights[i];

            if (randomValue < currentWeightSum)
            {
                return prefabs[i];
            }
        }

        return prefabs[0];
    }

    // Этот метод считает всех живых врагов.
    public int GetAliveEnemyCount()
    {
        EnemyHealth[] aliveEnemies = FindObjectsOfType<EnemyHealth>();
        return aliveEnemies.Length;
    }

    // Этот метод считает всех живых быстрых врагов по тегу.
    public int GetAliveFastEnemyCount()
    {
        GameObject[] fastEnemies = GameObject.FindGameObjectsWithTag("FastEnemy");
        return fastEnemies.Length;
    }

    // Этот метод считает всех живых громил по тегу.
    public int GetAliveHeavyEnemyCount()
    {
        GameObject[] heavyEnemies = GameObject.FindGameObjectsWithTag("HeavyEnemy");
        return heavyEnemies.Length;
    }

    // Этот метод меняет интервал спавна.
    public void SetSpawnInterval(float newSpawnInterval)
    {
        spawnInterval = Mathf.Max(0.2f, newSpawnInterval);
    }

    // Этот метод меняет максимум живых врагов.
    public void SetMaxAliveEnemies(int newMaxAliveEnemies)
    {
        maxAliveEnemies = Mathf.Max(1, newMaxAliveEnemies);
    }

    // Этот метод меняет лимит быстрых врагов.
    public void SetMaxAliveFastEnemies(int newMaxAliveFastEnemies)
    {
        maxAliveFastEnemies = Mathf.Max(0, newMaxAliveFastEnemies);
    }

    // Этот метод меняет лимит громил.
    public void SetMaxAliveHeavyEnemies(int newMaxAliveHeavyEnemies)
    {
        maxAliveHeavyEnemies = Mathf.Max(0, newMaxAliveHeavyEnemies);
    }

    // Этот метод включает или выключает быстрых врагов.
    public void SetFastEnemiesAllowed(bool isAllowed)
    {
        allowFastEnemies = isAllowed;
    }

    // Этот метод включает или выключает громил.
    public void SetHeavyEnemiesAllowed(bool isAllowed)
    {
        allowHeavyEnemies = isAllowed;
    }

    public float GetSpawnInterval()
    {
        return spawnInterval;
    }

    public int GetMaxAliveEnemies()
    {
        return maxAliveEnemies;
    }

    public int GetMaxAliveFastEnemies()
    {
        return maxAliveFastEnemies;
    }

    public int GetMaxAliveHeavyEnemies()
    {
        return maxAliveHeavyEnemies;
    }

    public void StartSpawning()
    {
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
}