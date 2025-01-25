using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all audio for the game using the Singleton pattern.
/// Handles background music and different types of dialogue sounds.
/// 
/// Key responsibilities:
/// - Controls background music playback
/// - Manages different typing sound effects for various message types
/// - Handles volume control for different audio types
/// 
/// Design patterns:
/// - Singleton: Ensures single point of audio control
/// - Strategy: Different sound strategies for different message types
/// </summary>
public class AudioManager : singleton<AudioManager>
{
    #region Serialized Fields
    [Header("Background Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip[] backgroundMusicPlaylist;
    [SerializeField] private AudioClip levelTransitionSound;
    [SerializeField] private float musicVolume = 0.5f;

    [Header("System Boot Message Sounds")]
    [SerializeField] private AudioClip[] bootMessageSounds;
    [SerializeField] private float bootSoundVolume = 0.4f;
    [SerializeField] [Range(0.8f, 1.2f)] private float bootPitchRange = 1.1f;

    [Header("Warning Message Sounds")]
    [SerializeField] private AudioClip warningSound;
    [SerializeField] private float warningSoundVolume = 0.5f;
    [SerializeField] [Range(0.8f, 1.2f)] private float warningPitchRange = 0.9f;

    [Header("Operator Message Sounds")]
    [SerializeField] private AudioClip[] operatorMessageSounds;
    [SerializeField] private float operatorSoundVolume = 0.3f;
    [SerializeField] [Range(0.8f, 1.2f)] private float operatorPitchRange = 1.05f;


    [Header("Response Click Sounds")]
    [SerializeField] private AudioClip responseClickSound;
    [SerializeField] private float responseVolume = 0.4f;
    [SerializeField] [Range(0.8f, 1.2f)] private float responsePitchRange = 1.05f;

    [Header("Game Over Sound")]
    [SerializeField] private AudioClip gameOversound;

    #endregion

    #region Private Fields
    private AudioSource musicSource;
    private AudioSource backgroundMusicSource;
    public AudioSource bootTypingSource;
    private AudioSource operatorTypingSource;
    private AudioSource responseSource;
    private AudioSource warningSource;
    private AudioSource levelTransitionSource;
    private AudioSource gameOverSource;
    private int currentBootSoundIndex = 0;
    private int currentOperatorSoundIndex = 0;
    private Coroutine currentMusicCoroutine;
    private int lastPlayedMusicIndex = -1;
    private List<int> playedMusicIndices = new List<int>();
    #endregion

    #region Unity Lifecycle
    public override void Awake()
    {
        base.Awake();
        InitializeAudioSources();
    }
    #endregion

    private void OnEnable()
    {
          GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
       GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

private void HandleGameStateChanged(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.idle:
                if (!backgroundMusicSource.isPlaying)
                {
                    currentMusicCoroutine = StartCoroutine(PlayNextRandomMusic());
                }
                break;

            case GameStates.levelIsChanging:
                PlayLevelTransitionSound();
                if (currentMusicCoroutine != null)
                {
                    StopCoroutine(currentMusicCoroutine);
                }
                currentMusicCoroutine = StartCoroutine(PlayNextRandomMusic());
                break;
            case GameStates.gameOver:
                StopAllGameplayMusic();
                gameOverSource.Play();
                break;    


        }
    }


    #region Initialization
    /// <summary>
    /// Initializes all audio sources with their respective settings
    /// </summary>
    private void InitializeAudioSources()
    {
        // Initialize music source
        musicSource = gameObject.AddComponent<AudioSource>();
        SetupMusicSource();

        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        SetupBackgroundMusicSource();

        gameOverSource = gameObject.AddComponent<AudioSource>();
        SetupGameOverSource();

        levelTransitionSource = gameObject.AddComponent<AudioSource>();

        // Initialize boot typing source
        bootTypingSource = gameObject.AddComponent<AudioSource>();
        SetupTypingSource(bootTypingSource, bootSoundVolume);

        // Initialize operator typing source
        operatorTypingSource = gameObject.AddComponent<AudioSource>();
        SetupTypingSource(operatorTypingSource, operatorSoundVolume);

        responseSource = gameObject.AddComponent<AudioSource>();
        SetupTypingSource(responseSource, responseVolume);

        warningSource = gameObject.AddComponent<AudioSource>();
        SetupTypingSource(warningSource, warningSoundVolume);
    }
    private void SetupBackgroundMusicSource()
    {
        backgroundMusicSource.loop = true;
        backgroundMusicSource.volume = musicVolume;
        backgroundMusicSource.playOnAwake = false;
    }


    /// <summary>
    /// Configures the background music audio source
    /// </summary>
    private void SetupMusicSource()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;
    }

    /// <summary>
    /// Configures a typing sound audio source with specific settings
    /// </summary>
    private void SetupTypingSource(AudioSource source, float volume)
    {
        source.playOnAwake = false;
        source.loop = false;
        source.volume = volume;
    }

    private void SetupGameOverSource()
    {
        gameOverSource.clip = gameOversound;
        gameOverSource.loop = false;
        gameOverSource.volume = 0.5f;
        gameOverSource.playOnAwake = false;
    }
    #endregion

    #region Background Music Controls
    /// <summary>
    /// Starts playing the background music if it's not already playing
    /// </summary>
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stops the currently playing background music
    /// </summary>
    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

private System.Collections.IEnumerator PlayNextRandomMusic()
    {
        if (backgroundMusicPlaylist == null || backgroundMusicPlaylist.Length == 0) yield break;

        // If we're in level transition, wait for the transition sound
        if (GameManager.instance.state == GameStates.levelIsChanging)
        {
            yield return new WaitForSeconds(levelTransitionSound ? levelTransitionSound.length : 0f);
        }

        // Reset playlist if all songs have been played
        if (playedMusicIndices.Count >= backgroundMusicPlaylist.Length)
        {
            playedMusicIndices.Clear();
            lastPlayedMusicIndex = -1;
        }

        int nextIndex;
        do
        {
            nextIndex = Random.Range(0, backgroundMusicPlaylist.Length);
        } while (playedMusicIndices.Contains(nextIndex) && backgroundMusicPlaylist.Length > 1);

        playedMusicIndices.Add(nextIndex);
        lastPlayedMusicIndex = nextIndex;

        // Stop any currently playing music before starting new one
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }

        backgroundMusicSource.clip = backgroundMusicPlaylist[nextIndex];
        backgroundMusicSource.Play();
    }

       public void StopAllGameplayMusic()
    {
        // Stop background music playlist
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }

        // Stop any running music coroutine
        if (currentMusicCoroutine != null)
        {
            StopCoroutine(currentMusicCoroutine);
            currentMusicCoroutine = null;
        }

        // Stop transition sound if playing
        if (levelTransitionSource != null)
        {
            levelTransitionSource.Stop();
        }

        // Clear the played indices to reset playlist for next game
        playedMusicIndices.Clear();
        lastPlayedMusicIndex = -1;
    }
      // This method can be called manually if needed
    public void StartBackgroundMusicPlaylist()
    {
        StopAllGameplayMusic(); // Clean up any existing music first
        if (!backgroundMusicSource.isPlaying)
        {
            currentMusicCoroutine = StartCoroutine(PlayNextRandomMusic());
        }
    }
    private void PlayLevelTransitionSound()
    {
        if (levelTransitionSource != null && levelTransitionSound != null)
        {
            backgroundMusicSource.Stop();
            levelTransitionSource.PlayOneShot(levelTransitionSound);
        }
    }





        public void PlayWarningSound()
    {
        if (warningSource != null && warningSound != null)
        {
            warningSource.clip = warningSound;
            warningSource.pitch = Random.Range(1f / warningPitchRange, warningPitchRange);
            warningSource.Play();
        }
    }


    /// <summary>
    /// Plays the response click sound with pitch variation
    /// </summary>
    public void PlayResponseClickSound()
    {
        if (responseSource != null && responseClickSound != null)
        {
            responseSource.clip = responseClickSound;
            responseSource.pitch = Random.Range(1f / responsePitchRange, responsePitchRange);
            responseSource.Play();
        }
    }
    /// <summary>
    /// Sets the volume of the background music
    /// </summary>
    /// <param name="volume">Volume value between 0 and 1</param>
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }
    #endregion

    #region Typing Sound Controls
    /// <summary>
    /// Plays a boot message typing sound with pitch variation
    /// </summary>
    public void PlayBootTypingSound()
    {
        if (bootTypingSource != null && bootMessageSounds != null && bootMessageSounds.Length > 0)
        {
            PlayTypingSound(bootTypingSource, bootMessageSounds, ref currentBootSoundIndex, bootPitchRange);
        }
    }

    /// <summary>
    /// Plays an operator message typing sound with pitch variation
    /// </summary>
    public void PlayOperatorTypingSound()
    {
        if (operatorTypingSource != null && operatorMessageSounds != null && operatorMessageSounds.Length > 0)
        {
            PlayTypingSound(operatorTypingSource, operatorMessageSounds, ref currentOperatorSoundIndex, operatorPitchRange);
        }
    }

    /// <summary>
    /// Generic method to play typing sound with specified parameters
    /// </summary>
    private void PlayTypingSound(AudioSource source, AudioClip[] sounds, ref int currentIndex, float pitchRange)
    {
        source.clip = sounds[currentIndex];
        currentIndex = (currentIndex + 1) % sounds.Length;
        source.pitch = Random.Range(1f / pitchRange, pitchRange);
        source.Play();
    }

    /// <summary>
    /// Sets the volume for boot message typing sounds
    /// </summary>
    public void SetBootTypingVolume(float volume)
    {
        if (bootTypingSource != null)
        {
            bootTypingSource.volume = Mathf.Clamp01(volume);
        }
    }
    /// <summary>
    /// Sets the volume for response click sounds
    /// </summary>
    public void SetResponseVolume(float volume)
    {
        if (responseSource != null)
        {
            responseSource.volume = Mathf.Clamp01(volume);
        }
    }
    public void SetWarningVolume(float volume)
    {
        if (warningSource != null)
        {
            warningSource.volume = Mathf.Clamp01(volume);
        }
    }

    /// <summary>
    /// Sets the volume for operator message typing sounds
    /// </summary>
    public void SetOperatorTypingVolume(float volume)
    {
        if (operatorTypingSource != null)
        {
            operatorTypingSource.volume = Mathf.Clamp01(volume);
        }
    }
    #endregion
}