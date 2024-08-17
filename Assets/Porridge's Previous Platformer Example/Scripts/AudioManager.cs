using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip jumpSound;
    public AudioClip flipSound;
    public AudioClip killSound;
    public AudioClip shootSound;
    public AudioClip landingSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 2f;
    }

    // play jump sound
    public void PlayJumpSound()
    {
        audioSource.clip = jumpSound;
        audioSource.Play();
        audioSource.PlayOneShot(jumpSound);
    }

    // play flip sound
    public void PlayFlipSound()
    {
        audioSource.PlayOneShot(flipSound);
    }

    // play kill sound
    public void PlayKillSound()
    {
        audioSource.PlayOneShot(killSound);
    }

    // play shoot sound
    public void PlayShootSound()
    {
        audioSource.PlayOneShot(shootSound);
    }

    // play landing sound
    public void PlayLandingSound()
    {
        audioSource.PlayOneShot(landingSound);
    }

}
