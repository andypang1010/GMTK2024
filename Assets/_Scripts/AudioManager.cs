using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sfx Clips")]
    public AudioClip walkClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip deathClip;
    public AudioClip scaleObjectClip;
    public AudioClip tagObjectClip;
    public AudioClip respawnClip;

    [Header("Music Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip LV1BGM;
    public AudioClip LV2BGM;
    public AudioClip LV3BGM;
    public AudioClip LV4BGM;

    [Header("Settings")]
    public float fadeSpeed;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MENU"
        || SceneManager.GetActiveScene().name == "TUTORIAL") {
            musicSource.clip = mainMenuBGM;
        }

        else if (SceneManager.GetActiveScene().name == "GAME") {
            switch (GameManager.Instance.currentLevel.name) {
                case "LV1_ENVIRONMENT":
                    FadeBGM(LV1BGM);
                    break;
                case "LV2_ENVIRONMENT":
                    FadeBGM(LV2BGM);
                    break;
                case "LV3_ENVIRONMENT":
                    FadeBGM(LV3BGM);
                    break;
                case "LV4_ENVIRONMENT":
                    FadeBGM(LV4BGM);
                    break;
                default:
                    musicSource.clip = null;
                    break;
            }

            if (GameManager.Instance.currentGameState == GameState.PAUSE) {
                musicSource.Pause();
                sfxSource.Pause();

                return;
            }
        }
        
        if (!musicSource.isPlaying) {
            musicSource.Play();
        }
    }

    void playWalk()
    {
        sfxSource.clip = walkClip;
        sfxSource.Play();
    }

    void playJump()
    {
        sfxSource.clip = jumpClip;
        sfxSource.PlayOneShot(jumpClip);
    }
    
    void FadeBGM(AudioClip clip) {
        if (musicSource.clip != clip) {
            StartCoroutine(Crossfade(musicSource, clip));
        }
    }

    IEnumerator Crossfade(AudioSource source, AudioClip targetClip) {
        while (source.volume > 0) {
            source.volume -= Time.deltaTime * fadeSpeed;
            source.volume = Mathf.Clamp(source.volume, 0, 1);
            yield return null;
        }

        source.Pause();
        source.clip = targetClip;
        source.Play();

        while (source.volume < 1) {
            source.volume += Time.deltaTime * fadeSpeed;
            source.volume = Mathf.Clamp(source.volume, 0, 1);
            yield return null;
        }
    }
}
