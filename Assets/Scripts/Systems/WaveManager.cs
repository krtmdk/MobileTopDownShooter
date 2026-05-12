using UnityEngine;

// Этот скрипт управляет волнами врагов.
// Баланс:
// 7 волн.
// Первая волна начинается с 20 убийств.
// Каждая следующая волна требует на 5 убийств больше.
// Быстрые враги появляются со 2 волны.
// Громила появляется с 4 волны.
// Одновременно на карте не должно быть слишком много врагов.
public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    // Спавнер врагов

    [SerializeField] private KillCounter killCounter;
    // Счётчик убийств

    [SerializeField] private VictoryUI victoryUI;
    // Панель победы

    [Header("Wave Settings")]
    [SerializeField] private int maxWaves = 7;
    // Общее количество волн

    [SerializeField]
    private int[] killsRequiredPerWave = new int[]
    {
        20, 25, 30, 35, 40, 45, 50
    };
    // Сколько убийств нужно сделать на каждой волне

    [SerializeField] private float intermissionDuration = 16f;
    // Передышка между волнами

    private int currentWave = 1;
    // Номер текущей волны

    private int killsRequired;
    // Сколько убийств нужно для текущей волны

    private int killsAtWaveStart;
    // Сколько убийств было на старте волны

    private bool isIntermission;
    // Идёт ли сейчас передышка

    private bool isWaveClearing;
    // Цель по убийствам выполнена, но на карте ещё остались враги

    private bool isMatchFinished;
    // Завершён ли матч

    private float intermissionTimer;
    // Таймер передышки

    private void Start()
    {
        StartWave();
    }

    private void Update()
    {
        if (isMatchFinished)
        {
            return;
        }

        if (isIntermission)
        {
            UpdateIntermission();
            return;
        }

        if (isWaveClearing)
        {
            UpdateWaveClearing();
            return;
        }

        UpdateWaveProgress();
    }

    private void StartWave()
    {
        isIntermission = false;
        isWaveClearing = false;

        killsAtWaveStart = killCounter.GetKillCount();
        killsRequired = GetKillsRequiredForWave(currentWave);

        ConfigureSpawnerForCurrentWave();

        enemySpawner.StartSpawning();

        Debug.Log("Wave " + currentWave + " started. Need kills: " + killsRequired);
    }

    private void ConfigureSpawnerForCurrentWave()
    {
        // Сначала выключаем особых врагов.
        // Потом включаем их только на нужных волнах.
        enemySpawner.SetFastEnemiesAllowed(false);
        enemySpawner.SetHeavyEnemiesAllowed(false);

        // Волна 1: разогрев, только обычные враги.
        if (currentWave == 1)
        {
            enemySpawner.SetSpawnInterval(1.45f);
            enemySpawner.SetMaxAliveEnemies(7);
            enemySpawner.SetMaxAliveFastEnemies(0);
            enemySpawner.SetMaxAliveHeavyEnemies(0);
            return;
        }

        // Волна 2: раннее появление быстрых врагов.
        if (currentWave == 2)
        {
            enemySpawner.SetFastEnemiesAllowed(true);

            enemySpawner.SetSpawnInterval(1.35f);
            enemySpawner.SetMaxAliveEnemies(8);
            enemySpawner.SetMaxAliveFastEnemies(1);
            enemySpawner.SetMaxAliveHeavyEnemies(0);
            return;
        }

        // Волна 3: быстрые враги уже становятся частью боя.
        if (currentWave == 3)
        {
            enemySpawner.SetFastEnemiesAllowed(true);

            enemySpawner.SetSpawnInterval(1.25f);
            enemySpawner.SetMaxAliveEnemies(9);
            enemySpawner.SetMaxAliveFastEnemies(2);
            enemySpawner.SetMaxAliveHeavyEnemies(0);
            return;
        }

        // Волна 4: первый громила.
        if (currentWave == 4)
        {
            enemySpawner.SetFastEnemiesAllowed(true);
            enemySpawner.SetHeavyEnemiesAllowed(true);

            enemySpawner.SetSpawnInterval(1.15f);
            enemySpawner.SetMaxAliveEnemies(10);
            enemySpawner.SetMaxAliveFastEnemies(2);
            enemySpawner.SetMaxAliveHeavyEnemies(1);
            return;
        }

        // Волна 5: стабильное давление быстрыми и громилой.
        if (currentWave == 5)
        {
            enemySpawner.SetFastEnemiesAllowed(true);
            enemySpawner.SetHeavyEnemiesAllowed(true);

            enemySpawner.SetSpawnInterval(1.05f);
            enemySpawner.SetMaxAliveEnemies(11);
            enemySpawner.SetMaxAliveFastEnemies(3);
            enemySpawner.SetMaxAliveHeavyEnemies(1);
            return;
        }

        // Волна 6: предфинальное давление.
        if (currentWave == 6)
        {
            enemySpawner.SetFastEnemiesAllowed(true);
            enemySpawner.SetHeavyEnemiesAllowed(true);

            enemySpawner.SetSpawnInterval(0.95f);
            enemySpawner.SetMaxAliveEnemies(12);
            enemySpawner.SetMaxAliveFastEnemies(3);
            enemySpawner.SetMaxAliveHeavyEnemies(1);
            return;
        }

        // Волна 7: финальная волна.
        enemySpawner.SetFastEnemiesAllowed(true);
        enemySpawner.SetHeavyEnemiesAllowed(true);

        enemySpawner.SetSpawnInterval(0.9f);
        enemySpawner.SetMaxAliveEnemies(13);
        enemySpawner.SetMaxAliveFastEnemies(3);
        enemySpawner.SetMaxAliveHeavyEnemies(2);
    }

    private int GetKillsRequiredForWave(int waveNumber)
    {
        int index = waveNumber - 1;

        if (killsRequiredPerWave != null && index >= 0 && index < killsRequiredPerWave.Length)
        {
            return killsRequiredPerWave[index];
        }

        return 20 + (waveNumber - 1) * 5;
    }

    private void UpdateWaveProgress()
    {
        int killsThisWave = GetKillsThisWave();

        if (killsThisWave >= killsRequired)
        {
            enemySpawner.StopSpawning();
            isWaveClearing = true;

            Debug.Log("Wave " + currentWave + " reached kill target. Clearing remaining enemies.");
        }
    }

    private void UpdateWaveClearing()
    {
        if (enemySpawner.GetAliveEnemyCount() > 0)
        {
            return;
        }

        StartIntermission();
    }

    private void StartIntermission()
    {
        isWaveClearing = false;

        if (currentWave >= maxWaves)
        {
            FinishMatchWithVictory();
            return;
        }

        isIntermission = true;
        intermissionTimer = intermissionDuration;

        Debug.Log("Wave " + currentWave + " completed. Intermission started.");
    }

    private void UpdateIntermission()
    {
        intermissionTimer -= Time.deltaTime;

        if (intermissionTimer > 0f)
        {
            return;
        }

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWave++;
        StartWave();
    }

    private void FinishMatchWithVictory()
    {
        isMatchFinished = true;
        isIntermission = false;
        isWaveClearing = false;

        enemySpawner.StopSpawning();

        Debug.Log("Match finished with victory.");

        if (victoryUI != null)
        {
            victoryUI.ShowVictory();
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public int GetMaxWaves()
    {
        return maxWaves;
    }

    public int GetKillsRequired()
    {
        return killsRequired;
    }

    public int GetKillsThisWave()
    {
        return killCounter.GetKillCount() - killsAtWaveStart;
    }

    public bool IsIntermission()
    {
        return isIntermission;
    }

    public bool IsWaveClearing()
    {
        return isWaveClearing;
    }

    public float GetIntermissionTime()
    {
        return intermissionTimer;
    }

    public bool IsMatchFinished()
    {
        return isMatchFinished;
    }
}