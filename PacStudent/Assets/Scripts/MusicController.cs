using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip initialClip;
    public AudioClip loopingClip;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        PlayInitialClip();
        
    }

    void PlayInitialClip()
    {
        audioSource.clip = initialClip;
        audioSource.loop = false;
        audioSource.Play();

        audioSource.PlayScheduled(AudioSettings.dspTime + initialClip.length);
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayLoopingClip();
        }
    }

    void PlayLoopingClip()
    {
        audioSource.clip = loopingClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
