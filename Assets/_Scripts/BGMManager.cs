using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{

    [Header("Audio Sources")]
    AudioSource bgmSource;

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
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        bgmSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MENU"
        || SceneManager.GetActiveScene().name == "OPENING CG"
        || SceneManager.GetActiveScene().name == "TUTORIAL") {
            bgmSource.clip = mainMenuBGM;

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
                    bgmSource.clip = null;
                    break;
            }

            if (GameManager.Instance.currentGameState == GameState.PAUSE) {
                bgmSource.Pause();
                return;
            }

            else {
                bgmSource.UnPause();
            }
        }
        
        if (!bgmSource.isPlaying) {
            bgmSource.Play();
        }
    }


    
    void FadeBGM(AudioClip clip) {
        if (bgmSource.clip == mainMenuBGM) {
            bgmSource.clip = clip;
            bgmSource.Play();
        }

        else if (bgmSource.clip != clip) {
            StartCoroutine(Crossfade(clip));
        }
    }

    IEnumerator Crossfade(AudioClip targetClip) {
        while (bgmSource.volume > 0) {
            bgmSource.volume -= Time.deltaTime * fadeSpeed;
            bgmSource.volume = Mathf.Clamp(bgmSource.volume, 0, 1);
            yield return null;
        }

        bgmSource.clip = targetClip;
        bgmSource.Play();

        while (bgmSource.volume < 1) {
            bgmSource.volume += Time.deltaTime * fadeSpeed;
            bgmSource.volume = Mathf.Clamp(bgmSource.volume, 0, 1);
            yield return null;
        }
    }

    public void ResetAudio() {
        bgmSource.clip = null;
    }
}
