using UnityEngine;

public class GameplayMusicManager : MonoBehaviour
{
    private enum GameplayMusicState
    {
        None,
        Ambient,
        Battle,
        FinalBattle
    }

    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    // Менеджер волн

    [SerializeField] private PauseMenuUI pauseMenuUI;
    // Меню паузы

    [Header("Audio Sources")]
    [SerializeField] private AudioSource gameplaySource;
    // Основная музыка игры: бой, перерыв и финальная волна

    [SerializeField] private AudioSource pauseSource;
    // Отдельная музыка паузы

    [Header("Music Lists")]
    [SerializeField] private AudioClip[] ambientMusicList;
    // Музыка между волнами

    [SerializeField] private AudioClip[] battleMusicList;
    // Обычные боевые треки

    [SerializeField] private AudioClip finalBattleMusic;
    // Отдельный трек только для финальной волны

    [SerializeField] private AudioClip pauseMusic;
    // Музыка паузы

    [Header("Volume")]
    [SerializeField] private float ambientVolume = 0.3f;
    // Громкость музыки между волнами

    [SerializeField] private float battleVolume = 0.45f;
    // Громкость обычной боевой музыки

    [SerializeField] private float finalBattleVolume = 0.5f;
    // Громкость финальной боевой музыки

    [SerializeField] private float pauseVolume = 0.25f;
    // Громкость музыки паузы

    private GameplayMusicState currentState = GameplayMusicState.None;
    // Текущее музыкальное состояние

    private AudioClip currentGameplayClip;
    // Текущий основной трек

    private bool wasPaused;
    // Была ли игра на паузе в прошлом кадре

    private void Awake()
    {
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        if (pauseMenuUI == null)
        {
            pauseMenuUI = FindObjectOfType<PauseMenuUI>();
        }

        SetupAudioSources();
    }

    private void Update()
    {
        UpdatePauseMusic();

        if (IsPaused())
        {
            return;
        }

        UpdateGameplayMusic();
    }

    private void SetupAudioSources()
    {
        if (gameplaySource == null)
        {
            gameplaySource = gameObject.AddComponent<AudioSource>();
        }

        if (pauseSource == null)
        {
            pauseSource = gameObject.AddComponent<AudioSource>();
        }

        gameplaySource.loop = true;
        gameplaySource.playOnAwake = false;
        gameplaySource.spatialBlend = 0f;

        pauseSource.loop = true;
        pauseSource.playOnAwake = false;
        pauseSource.spatialBlend = 0f;
        pauseSource.clip = pauseMusic;
        pauseSource.volume = pauseVolume;
    }

    private void UpdatePauseMusic()
    {
        bool isPausedNow = IsPaused();

        // Если игрок только что нажал паузу
        if (isPausedNow && !wasPaused)
        {
            if (gameplaySource != null && gameplaySource.isPlaying)
            {
                gameplaySource.Pause();
            }

            if (pauseSource != null && pauseMusic != null)
            {
                pauseSource.clip = pauseMusic;
                pauseSource.volume = pauseVolume;
                pauseSource.Play();
            }
        }

        // Если игрок только что снял паузу
        if (!isPausedNow && wasPaused)
        {
            if (pauseSource != null)
            {
                pauseSource.Stop();
            }

            if (gameplaySource != null && gameplaySource.clip != null)
            {
                gameplaySource.UnPause();
            }
        }

        wasPaused = isPausedNow;
    }

    private void UpdateGameplayMusic()
    {
        // При победе или поражении текущая музыка продолжает играть
        if (waveManager != null && waveManager.IsMatchFinished())
        {
            return;
        }

        // Между волнами играет ambient
        if (waveManager != null && waveManager.IsIntermission())
        {
            TryPlayGameplayMusic(GameplayMusicState.Ambient, ambientMusicList, ambientVolume);
            return;
        }

        // На финальной волне играет отдельный финальный трек
        if (IsFinalWave())
        {
            TryPlayFinalBattleMusic();
            return;
        }

        // На обычных волнах играет обычный боевой трек
        TryPlayGameplayMusic(GameplayMusicState.Battle, battleMusicList, battleVolume);
    }

    private bool IsFinalWave()
    {
        if (waveManager == null)
        {
            return false;
        }

        return waveManager.GetCurrentWave() >= waveManager.GetMaxWaves();
    }

    private void TryPlayFinalBattleMusic()
    {
        if (currentState == GameplayMusicState.FinalBattle)
        {
            if (gameplaySource != null)
            {
                gameplaySource.volume = finalBattleVolume;
            }

            return;
        }

        if (finalBattleMusic == null)
        {
            TryPlayGameplayMusic(GameplayMusicState.Battle, battleMusicList, battleVolume);
            return;
        }

        currentState = GameplayMusicState.FinalBattle;
        currentGameplayClip = finalBattleMusic;

        gameplaySource.Stop();
        gameplaySource.clip = currentGameplayClip;
        gameplaySource.volume = finalBattleVolume;
        gameplaySource.Play();
    }

    private void TryPlayGameplayMusic(GameplayMusicState newState, AudioClip[] clipList, float volume)
    {
        if (currentState == newState)
        {
            if (gameplaySource != null)
            {
                gameplaySource.volume = volume;
            }

            return;
        }

        if (clipList == null || clipList.Length == 0)
        {
            return;
        }

        AudioClip selectedClip = GetRandomClip(clipList);

        if (selectedClip == null)
        {
            return;
        }

        currentState = newState;
        currentGameplayClip = selectedClip;

        gameplaySource.Stop();
        gameplaySource.clip = currentGameplayClip;
        gameplaySource.volume = volume;
        gameplaySource.Play();
    }

    private AudioClip GetRandomClip(AudioClip[] clipList)
    {
        if (clipList.Length == 1)
        {
            return clipList[0];
        }

        AudioClip selectedClip = null;

        for (int i = 0; i < 10; i++)
        {
            selectedClip = clipList[Random.Range(0, clipList.Length)];

            if (selectedClip != currentGameplayClip)
            {
                return selectedClip;
            }
        }

        return selectedClip;
    }

    private bool IsPaused()
    {
        if (pauseMenuUI == null)
        {
            return false;
        }

        return pauseMenuUI.IsPaused();
    }
}