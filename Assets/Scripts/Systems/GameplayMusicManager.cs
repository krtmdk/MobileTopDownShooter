using UnityEngine;

public class GameplayMusicManager : MonoBehaviour
{
    private enum GameplayMusicState
    {
        None,
        Ambient,
        Battle
    }

    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private PauseMenuUI pauseMenuUI;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource gameplaySource;
    // Основная музыка игры: бой и перерыв между волнами.

    [SerializeField] private AudioSource pauseSource;
    // Отдельный источник музыки паузы.

    [Header("Music Lists")]
    [SerializeField] private AudioClip[] ambientMusicList;
    // Музыка между волнами.

    [SerializeField] private AudioClip[] battleMusicList;
    // Боевые треки.

    [SerializeField] private AudioClip pauseMusic;
    // Музыка паузы.

    [Header("Volume")]
    [SerializeField] private float ambientVolume = 0.3f;
    [SerializeField] private float battleVolume = 0.45f;
    [SerializeField] private float pauseVolume = 0.25f;

    private GameplayMusicState currentState = GameplayMusicState.None;
    private AudioClip currentGameplayClip;
    private bool wasPaused;

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

        // Только что поставили паузу.
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

        // Только что сняли паузу.
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
        // При победе или поражении текущая музыка продолжает играть.
        if (waveManager != null && waveManager.IsMatchFinished())
        {
            return;
        }

        if (waveManager != null && waveManager.IsIntermission())
        {
            TryPlayGameplayMusic(GameplayMusicState.Ambient, ambientMusicList, ambientVolume);
            return;
        }

        TryPlayGameplayMusic(GameplayMusicState.Battle, battleMusicList, battleVolume);
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