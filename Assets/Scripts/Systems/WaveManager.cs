using UnityEngine;

// Этот скрипт управляет волнами.
// В этой версии:
// 1-2 волна: только обычные
// 3-4 волна: обычные + быстрые
// 5-6 волна: обычные + быстрые + громилы
public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    // Спавнер врагов.

    [SerializeField] private KillCounter killCounter;
    // Счётчик убийств.

    [SerializeField] private VictoryUI victoryUI;
    // UI победы.

    [Header("Wave Settings")]
    [SerializeField] private int maxWaves = 5;
    // Общее количество волн.

    [SerializeField] private int[] killsRequiredPerWave = new int[] { 12, 18, 24, 30, 35 };
    // Сколько убийств нужно для каждой волны.
    // 5 волн: коротко, понятно, нормально для защиты.

    [SerializeField] private float intermissionDuration = 12f;
    // Передышка между волнами.

    private int currentWave = 1;
    // Текущий номер волны.

    private int killsRequired;
    // Сколько убийств нужно для текущей волны.

    private int killsAtWaveStart;
    // Сколько убийств было на старте текущей волны.

    private bool isIntermission;
    // Идёт ли сейчас передышка.

    private bool isWaveClearing;
    // Волна уже выполнена по убийствам, но на карте ещё есть враги.

    private bool isMatchFinished;
    // Матч полностью завершён.

    private float intermissionTimer;
    // Таймер передышки.

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

    // Этот метод запускает текущую волну.
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

    // Этот метод настраивает спавнер под текущую волну.
    private void ConfigureSpawnerForCurrentWave()
    {
        // Сначала выключаем опасных врагов.
        // Потом ниже включаем их только на нужных волнах.
        enemySpawner.SetFastEnemiesAllowed(false);
        enemySpawner.SetHeavyEnemiesAllowed(false);

        // Волна 1: спокойное начало, только обычные враги.
        if (currentWave == 1)
        {
            enemySpawner.SetSpawnInterval(1.6f);
            enemySpawner.SetMaxAliveEnemies(6);
            enemySpawner.SetMaxAliveFastEnemies(0);
            enemySpawner.SetMaxAliveHeavyEnemies(0);
            return;
        }

        // Волна 2: больше обычных врагов, но без быстрых.
        if (currentWave == 2)
        {
            enemySpawner.SetSpawnInterval(1.4f);
            enemySpawner.SetMaxAliveEnemies(8);
            enemySpawner.SetMaxAliveFastEnemies(0);
            enemySpawner.SetMaxAliveHeavyEnemies(0);
            return;
        }

        // Волна 3: появляются быстрые враги.
        if (currentWave == 3)
        {
            enemySpawner.SetFastEnemiesAllowed(true);

            enemySpawner.SetSpawnInterval(1.3f);
            enemySpawner.SetMaxAliveEnemies(9);
            enemySpawner.SetMaxAliveFastEnemies(2);
            enemySpawner.SetMaxAliveHeavyEnemies(0);
            return;
        }

        // Волна 4: быстрых врагов становится больше, давление растёт.
        if (currentWave == 4)
        {
            enemySpawner.SetFastEnemiesAllowed(true);

            enemySpawner.SetSpawnInterval(1.15f);
            enemySpawner.SetMaxAliveEnemies(11);
            enemySpawner.SetMaxAliveFastEnemies(3);
            enemySpawner.SetMaxAliveHeavyEnemies(0);
            return;
        }

        // Волна 5: финальная волна, обычные + быстрые + громила.
        enemySpawner.SetFastEnemiesAllowed(true);
        enemySpawner.SetHeavyEnemiesAllowed(true);

        enemySpawner.SetSpawnInterval(1.0f);
        enemySpawner.SetMaxAliveEnemies(12);
        enemySpawner.SetMaxAliveFastEnemies(3);
        enemySpawner.SetMaxAliveHeavyEnemies(1);
    }

    // Этот метод возвращает цель по убийствам для указанной волны.
    private int GetKillsRequiredForWave(int waveNumber)
    {
        int index = waveNumber - 1;

        if (killsRequiredPerWave != null && index >= 0 && index < killsRequiredPerWave.Length)
        {
            return killsRequiredPerWave[index];
        }

        // Запасной вариант, если массив вдруг не заполнен.
        return 50 + (waveNumber - 1) * 5;
    }

    // Этот метод проверяет, выполнена ли цель по убийствам.
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

    // Этот метод ждёт, пока игрок добьёт оставшихся врагов.
    private void UpdateWaveClearing()
    {
        if (enemySpawner.GetAliveEnemyCount() > 0)
        {
            return;
        }

        StartIntermission();
    }

    // Этот метод запускает передышку между волнами.
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

    // Этот метод обновляет таймер передышки.
    private void UpdateIntermission()
    {
        intermissionTimer -= Time.deltaTime;

        if (intermissionTimer > 0f)
        {
            return;
        }

        StartNextWave();
    }

    // Этот метод запускает следующую волну.
    private void StartNextWave()
    {
        currentWave++;
        StartWave();
    }

    // Этот метод завершает матч победой.
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