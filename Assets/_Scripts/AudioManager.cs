using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public AudioClip gameBGM;


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

    }
}
