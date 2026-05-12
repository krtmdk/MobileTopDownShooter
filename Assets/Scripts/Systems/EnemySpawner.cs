using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Этот скрипт отвечает за спавн врагов.
// Перед созданием врага он ищет ближайшую валидную точку NavMesh,
// чтобы враг не появлялся вне навигационной зоны.
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject fastEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxAliveEnemies = 8;

    [Header("NavMesh Spawn Safety")]
    [SerializeField] private float navMeshSearchRadius = 4f;
    // Радиус поиска ближайшей точки NavMesh вокруг SpawnPoint

    [Header("Dangerous Enemy Limits")]
    [SerializeField] private int maxAliveFastEnemies = 2;
    [SerializeField] private int maxAliveHeavyEnemies = 1;

    [Header("Wave Type Flags")]
    [SerializeField] private bool allowFastEnemies = false;
    [SerializeField] private bool allowHeavyEnemies = false;

    [Header("Spawn Weights")]
    [SerializeField] private int normalEnemyWeight = 100;
    [SerializeField] private int fastEnemyWeight = 35;
    [SerializeField] private int heavyEnemyWeight = 15;

    private float spawnTimer;
    private bool isSpawning = true;

    private void Start()
    {
        spawnTimer = 0f;
    }

    private void Update()
    {
        UpdateSpawnTimer();
    }

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

    private void SpawnEnemy()
    {
        GameObject selectedEnemyPrefab = GetEnemyPrefabForCurrentWave();

        if (selectedEnemyPrefab == null)
        {
            return;
        }

        Transform selectedSpawnPoint = GetRandomSpawnPoint();

        if (selectedSpawnPoint == null)
        {
            return;
        }

        Vector3 spawnPosition;

        bool hasValidPosition = TryGetNavMeshSpawnPosition(
            selectedSpawnPoint.position,
            out spawnPosition
        );

        if (!hasValidPosition)
        {
            Debug.LogWarning("Spawn point is not near NavMesh: " + selectedSpawnPoint.name);
            return;
        }

        GameObject spawnedEnemy = Instantiate(
            selectedEnemyPrefab,
            spawnPosition,
            selectedSpawnPoint.rotation
        );

        NavMeshAgent agent = spawnedEnemy.GetComponent<NavMeshAgent>();

        if (agent != null && agent.enabled)
        {
            agent.Warp(spawnPosition);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            return null;
        }

        int randomSpawnIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomSpawnIndex];
    }

    private bool TryGetNavMeshSpawnPosition(Vector3 sourcePosition, out Vector3 spawnPosition)
    {
        NavMeshHit hit;

        bool foundPosition = NavMesh.SamplePosition(
            sourcePosition,
            out hit,
            navMeshSearchRadius,
            NavMesh.AllAreas
        );

        if (foundPosition)
        {
            spawnPosition = hit.position;
            return true;
        }

        spawnPosition = sourcePosition;
        return false;
    }

    private GameObject GetEnemyPrefabForCurrentWave()
    {
        List<GameObject> availablePrefabs = new List<GameObject>();
        List<int> availableWeights = new List<int>();

        if (normalEnemyPrefab != null)
        {
            availablePrefabs.Add(normalEnemyPrefab);
            availableWeights.Add(Mathf.Max(1, normalEnemyWeight));
        }

        if (allowFastEnemies && fastEnemyPrefab != null)
        {
            if (GetAliveFastEnemyCount() < maxAliveFastEnemies)
            {
                availablePrefabs.Add(fastEnemyPrefab);
                availableWeights.Add(Mathf.Max(1, fastEnemyWeight));
            }
        }

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

    public int GetAliveEnemyCount()
    {
        EnemyHealth[] aliveEnemies = FindObjectsOfType<EnemyHealth>();
        return aliveEnemies.Length;
    }

    public int GetAliveFastEnemyCount()
    {
        GameObject[] fastEnemies = GameObject.FindGameObjectsWithTag("FastEnemy");
        return fastEnemies.Length;
    }

    public int GetAliveHeavyEnemyCount()
    {
        GameObject[] heavyEnemies = GameObject.FindGameObjectsWithTag("HeavyEnemy");
        return heavyEnemies.Length;
    }

    public void SetSpawnInterval(float newSpawnInterval)
    {
        spawnInterval = Mathf.Max(0.2f, newSpawnInterval);
    }

    public void SetMaxAliveEnemies(int newMaxAliveEnemies)
    {
        maxAliveEnemies = Mathf.Max(1, newMaxAliveEnemies);
    }

    public void SetMaxAliveFastEnemies(int newMaxAliveFastEnemies)
    {
        maxAliveFastEnemies = Mathf.Max(0, newMaxAliveFastEnemies);
    }

    public void SetMaxAliveHeavyEnemies(int newMaxAliveHeavyEnemies)
    {
        maxAliveHeavyEnemies = Mathf.Max(0, newMaxAliveHeavyEnemies);
    }

    public void SetFastEnemiesAllowed(bool isAllowed)
    {
        allowFastEnemies = isAllowed;
    }

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