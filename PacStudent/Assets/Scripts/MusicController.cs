using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip normalMusic;
    public AudioClip hyperMusic;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        PlayNormalMusic();
        
    }

    public void PlayNormalMusic()
    {
        audioSource.clip = normalMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayHyperMusic()
    {
        audioSource.clip = hyperMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
