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



    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private float globalMasterVolume = 1f;
    private float masterMusicVolume = 1f;
    private float masterSFXVolume = 1f;

    public bool AudioExisted = false;
    public GameObject ExistedAudio;

    [Header("Global Music & Sounds")]
    [SerializeField] public AudioClip music;
    [SerializeField, Range(0f, 2f)] public float musicVolume = 0.5f;

    [SerializeField] private AudioClip movement;
    [SerializeField, Range(0f, 2f)] private float movementVolume = 1f;

    [SerializeField] private AudioClip headbutt;
    [SerializeField, Range(0f, 2f)] private float headbuttVolume = 1f;

    [SerializeField] private AudioClip boardtilting;
    [SerializeField, Range(0f, 2f)] private float boardtiltingVolume = 1f;

    [SerializeField] private AudioClip enemySpawn;
    [SerializeField, Range(0f, 2f)] private float enemySpawnVolume = 1f;

    [SerializeField] private AudioClip TakingDamage;
    [SerializeField, Range(0f, 2f)] private float TakingDamageVolume = 1f;

    [SerializeField] private AudioClip EnemyDie;
    [SerializeField, Range(0f, 2f)] private float EnemyDieVolume = 1f;

    [SerializeField] private AudioClip GameOver;
    [SerializeField, Range(0f, 2f)] private float GameOverVolume = 1f;

    [SerializeField] private AudioClip GameWin;
    [SerializeField, Range(0f, 2f)] private float GameWinVolume = 1f;


    [Header("Upgrades & UI")]
    [SerializeField] private AudioClip UpgradeButton;
    [SerializeField, Range(0f, 2f)] private float UpgradeButtonVolume = 1f;

    [SerializeField] private AudioClip HoverSound;
    [SerializeField, Range(0f, 2f)] private float HoverSoundVolume = 1f;

    [SerializeField] private AudioClip Buttons;
    [SerializeField, Range(0f, 2f)] private float ButtonsVolume = 1f;

    [SerializeField] public AudioClip waveTransition;
    [SerializeField, Range(0f, 2f)] public float waveTransitionVolume = 1f;

    [SerializeField] private AudioClip Abilities;
    [SerializeField, Range(0f, 2f)] private float AbilitiesVolume = 1f;


    [Header("Enemies")]
    [SerializeField] private AudioClip HandPoke;
    [SerializeField, Range(0f, 2f)] private float HandPokeVolume = 1f;

    [SerializeField] private AudioClip HandShoot;
    [SerializeField, Range(0f, 2f)] private float HandShootVolume = 1f;

    [SerializeField] private AudioClip HandLazer;
    [SerializeField, Range(0f, 2f)] private float HandLazerVolume = 1f;

    [SerializeField] private AudioClip HandLazer2;
    [SerializeField, Range(0f, 2f)] private float HandLazerVolume2 = 1f;

    [SerializeField] private AudioClip explosion;
    [SerializeField, Range(0f, 2f)] private float explosionVolume = 1f;

    [SerializeField] private AudioClip blocktoss;
    [SerializeField, Range(0f, 2f)] private float blocktossVolume = 1f;

    [SerializeField] private AudioClip blockPlace;
    [SerializeField, Range(0f, 2f)] private float blockPlaceVolume = 1f;



    private void Awake()
    {
        Instance = this;
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
            playMusic(music, musicVolume);
        }
    }

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


    public void playMusic(AudioClip clip, float volume = 1f)
    {
        bgMusic.clip = clip;
        bgMusic.volume = volume * masterMusicVolume * globalMasterVolume;
        bgMusic.loop = true;
        bgMusic.Play();
    }

    public void playUISound(AudioClip clip, float volume = 1f)
    {
        if (clip != null) soundEffectforUI.PlayOneShot(clip, volume * masterSFXVolume * globalMasterVolume);
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, position, volume * masterSFXVolume * globalMasterVolume);
    }

    public void PlayTrimmedAudio(AudioClip clip, Vector3 position, float duration, float volume = 1f)
    {
        if (clip != null) StartCoroutine(TrimmedAudio(clip, position, duration, volume * masterSFXVolume * globalMasterVolume));
    }

    IEnumerator TrimmedAudio(AudioClip clip, Vector3 position, float duration, float volume = 1f)
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


    public void PlayMovementSound(Vector3 position) => PlaySFX(movement, position, movementVolume);
    public void PlayHeadbuttSound(Vector3 position) => PlaySFX(headbutt, position, headbuttVolume);
    public void PlayBoardTiltingSound(Vector3 position) => PlaySFX(boardtilting, position, boardtiltingVolume);
    public void PlayTakingDamageSound(Vector3 position) => PlaySFX(TakingDamage, position, TakingDamageVolume);

    public void PlayEnemySpawnSound(Vector3 position) => PlaySFX(enemySpawn, position, enemySpawnVolume);
    public void PlayEnemyDieSound(Vector3 position) => PlaySFX(EnemyDie, position, EnemyDieVolume);
    public void PlayGameOverSound() => playUISound(GameOver, GameOverVolume);
    public void PlayGameWinSound() => playUISound(GameWin, GameWinVolume);

    public void PlayUpgradeButtonSound() => playUISound(UpgradeButton, UpgradeButtonVolume);
    public void PlayHoverSound() => playUISound(HoverSound, HoverSoundVolume);

    public void PlayGenericButtonSound() => playUISound(Buttons, ButtonsVolume);
    public void PlayAbilitiesSound(Vector3 position, float duration) => PlayTrimmedAudio(Abilities, position, duration, AbilitiesVolume);
    public void PlayWaveTransitionSound() => playUISound(waveTransition, waveTransitionVolume);
    public void PlayHandPokeSound(Vector3 position) => PlaySFX(HandPoke, position, HandPokeVolume);
    public void PlayHandShootSound(Vector3 position) => PlaySFX(HandShoot, position, HandShootVolume);
    public void PlayHandLazerSound(Vector3 position) => PlaySFX(HandLazer, position, HandLazerVolume);
    public void PlayHandLazerSoundDuring(Vector3 position) => PlaySFX(HandLazer2, position, HandLazerVolume2);

    public void PlayExplosionSound(Vector3 position) => PlaySFX(explosion, position, explosionVolume);
    public void PlayBlockTossSound(Vector3 position) => PlaySFX(blocktoss, position, blocktossVolume);
    public void PlayBlockPlaceSound(Vector3 position) => PlaySFX(blockPlace, position, blockPlaceVolume);
}