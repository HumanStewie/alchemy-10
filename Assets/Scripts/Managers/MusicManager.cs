using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioSource soundEffectforUI;

    [Header("UI Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private float globalMasterVolume = 1f;
    private float masterMusicVolume = 1f;
    private float masterSFXVolume = 1f;

    public bool AudioExisted = false;
    public GameObject ExistedAudio;

    [Header("BGM & Game State")]
    [SerializeField] public AudioClip music;
    [SerializeField, Range(0f, 2f)] public float musicVolume = 0.5f;

    [SerializeField] private AudioClip gameWin;
    [SerializeField, Range(0f, 2f)] private float gameWinVolume = 1f;

    [SerializeField] private AudioClip gameOver;
    [SerializeField, Range(0f, 2f)] private float gameOverVolume = 1f;

    [Header("Player & Combat Movement")]
    [SerializeField] private AudioClip walkingSound;
    [SerializeField, Range(0f, 2f)] private float walkingSoundVolume = 1f;

    [SerializeField] private AudioClip playerHurt;
    [SerializeField, Range(0f, 2f)] private float playerHurtVolume = 1f;

    [SerializeField] private AudioClip enemyHurt;
    [SerializeField, Range(0f, 2f)] private float enemyHurtVolume = 1f;

    [SerializeField] private AudioClip dieSound;
    [SerializeField, Range(0f, 2f)] private float dieSoundVolume = 1f;

    [Header("Gesture & Rune Drawing")]
    [SerializeField] private AudioClip handDrawMode;
    [SerializeField, Range(0f, 2f)] private float handDrawModeVolume = 1f;

    [SerializeField] private AudioClip drawSound;
    [SerializeField, Range(0f, 2f)] private float drawSoundVolume = 1f;

    [SerializeField] private AudioClip runeMovement;
    [SerializeField, Range(0f, 2f)] private float runeMovementVolume = 1f;

    [SerializeField] private AudioClip spellRecognized;
    [SerializeField, Range(0f, 2f)] private float spellRecognizedVolume = 1f;

    [Header("Spells & Attacks")]
    [SerializeField] private AudioClip swordSound;
    [SerializeField, Range(0f, 2f)] private float swordSoundVolume = 1f;

    [SerializeField] private AudioClip createWallSound;
    [SerializeField, Range(0f, 2f)] private float createWallSoundVolume = 1f;

    [SerializeField] private AudioClip spreadSound;
    [SerializeField, Range(0f, 2f)] private float spreadSoundVolume = 1f;

    [SerializeField] private AudioClip bazookaSound;
    [SerializeField, Range(0f, 2f)] private float bazookaSoundVolume = 1f;

    [SerializeField] private AudioClip jamHitSound;
    [SerializeField, Range(0f, 2f)] private float jamHitSoundVolume = 1f;

    [SerializeField] private AudioClip eatingSound;
    [SerializeField, Range(0f, 2f)] private float eatingSoundVolume = 1f;

    [Header("Enemy Types & Actions")]
    [SerializeField] private AudioClip simpleFollower;
    [SerializeField, Range(0f, 2f)] private float simpleFollowerVolume = 1f;

    [SerializeField] private AudioClip wallSpawnerSound;
    [SerializeField, Range(0f, 2f)] private float wallSpawnerSoundVolume = 1f;

    [SerializeField] private AudioClip butterShooter;
    [SerializeField, Range(0f, 2f)] private float butterShooterVolume = 1f;

    [SerializeField] private AudioClip sniper;
    [SerializeField, Range(0f, 2f)] private float sniperVolume = 1f;

    [SerializeField] private AudioClip jamToucher;
    [SerializeField, Range(0f, 2f)] private float jamToucherVolume = 1f;

    [SerializeField] private AudioClip charger;
    [SerializeField, Range(0f, 2f)] private float chargerVolume = 1f;

    [Header("UI Interactions")]
    [SerializeField] private AudioClip buttonHovering;
    [SerializeField, Range(0f, 2f)] private float buttonHoveringVolume = 1f;

    [SerializeField] private AudioClip buttonClick;
    [SerializeField, Range(0f, 2f)] private float buttonClickVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        globalMasterVolume = PlayerPrefs.GetFloat("GlobalMasterVolume", 1f);
        masterMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        masterSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (masterSlider != null)
        {
            masterSlider.value = globalMasterVolume;
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.value = masterMusicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = masterSFXVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayMusic(music, musicVolume);
        }
    }

    // --- Volume Setters ---
    public void SetMasterVolume(float volume)
    {
        globalMasterVolume = volume;
        if (bgMusic != null) bgMusic.volume = musicVolume * masterMusicVolume * globalMasterVolume;
        PlayerPrefs.SetFloat("GlobalMasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        masterMusicVolume = volume;
        if (bgMusic != null) bgMusic.volume = musicVolume * masterMusicVolume * globalMasterVolume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        masterSFXVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // --- Base Player Methods ---
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null || bgMusic == null) return;
        bgMusic.clip = clip;
        bgMusic.volume = volume * masterMusicVolume * globalMasterVolume;
        bgMusic.loop = true;
        bgMusic.Play();
    }

    public void PlayUISound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && soundEffectforUI != null)
        {
            soundEffectforUI.PlayOneShot(clip, volume * masterSFXVolume * globalMasterVolume);
        }
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume * masterSFXVolume * globalMasterVolume);
        }
    }

    public void PlayTrimmedAudio(AudioClip clip, Vector3 position, float duration, float volume = 1f)
    {
        if (clip != null)
        {
            StartCoroutine(TrimmedAudio(clip, position, duration, volume * masterSFXVolume * globalMasterVolume));
        }
    }

    private IEnumerator TrimmedAudio(AudioClip clip, Vector3 position, float duration, float volume = 1f)
    {
        var tempSound = new GameObject("TempSound");
        tempSound.transform.position = position;

        var audioSource = tempSound.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;

        audioSource.Play();
        yield return new WaitForSeconds(duration);

        Destroy(tempSound);
    }

    public GameObject PlayCanBeDestroyedAudio(AudioClip clip, Vector3 position, float volume = 1f)
    {
        var tempSound = new GameObject("TempSound");
        tempSound.transform.position = position;

        var audioSource = tempSound.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;

        audioSource.Play();
        AudioExisted = true;
        ExistedAudio = tempSound;
        return tempSound;
    }

    // --- Game State & UI Callbacks ---
    public void PlayGameWinSound() => PlayUISound(gameWin, gameWinVolume);
    public void PlayGameOverSound() => PlayUISound(gameOver, gameOverVolume);
    public void PlayButtonHoveringSound() => PlayUISound(buttonHovering, buttonHoveringVolume);
    public void PlayButtonClickSound() => PlayUISound(buttonClick, buttonClickVolume);

    // --- Movement & Damage Callbacks ---
    public void PlayWalkingSound(Vector3 position) => PlaySFX(walkingSound, position, walkingSoundVolume);
    public void PlayPlayerHurtSound(Vector3 position) => PlaySFX(playerHurt, position, playerHurtVolume);
    public void PlayEnemyHurtSound(Vector3 position) => PlaySFX(enemyHurt, position, enemyHurtVolume);
    public void PlayDieSound(Vector3 position) => PlaySFX(dieSound, position, dieSoundVolume);

    public void PlayHandDrawModeSound(Vector3 position) => PlaySFX(handDrawMode, position, handDrawModeVolume);
    public void PlayDrawSound(Vector3 position) => PlaySFX(drawSound, position, drawSoundVolume);
    public void PlayRuneMovementSound(Vector3 position) => PlaySFX(runeMovement, position, runeMovementVolume);
    public void PlaySpellRecognizedSound(Vector3 position) => PlaySFX(spellRecognized, position, spellRecognizedVolume);

    // --- Spells & Attack Callbacks ---
    public void PlaySwordSound(Vector3 position) => PlaySFX(swordSound, position, swordSoundVolume);
    public void PlayCreateWallSound(Vector3 position) => PlaySFX(createWallSound, position, createWallSoundVolume);
    public void PlaySpreadSound(Vector3 position) => PlaySFX(spreadSound, position, spreadSoundVolume);
    public void PlayBazookaSound(Vector3 position) => PlaySFX(bazookaSound, position, bazookaSoundVolume);
    public void PlayJamHitSound(Vector3 position) => PlaySFX(jamHitSound, position, jamHitSoundVolume);
    public void PlayEatingSound(Vector3 position) => PlaySFX(eatingSound, position, eatingSoundVolume);

    // --- Enemy Callbacks ---
    public void PlaySimpleFollowerSound(Vector3 position) => PlaySFX(simpleFollower, position, simpleFollowerVolume);
    public void PlayWallSpawnerSound(Vector3 position) => PlaySFX(wallSpawnerSound, position, wallSpawnerSoundVolume);
    public void PlayButterShooterSound(Vector3 position) => PlaySFX(butterShooter, position, butterShooterVolume);
    public void PlaySniperSound(Vector3 position) => PlaySFX(sniper, position, sniperVolume);
    public void PlayJamToucherSound(Vector3 position) => PlaySFX(jamToucher, position, jamToucherVolume);
    public void PlayChargerSound(Vector3 position) => PlaySFX(charger, position, chargerVolume);
}