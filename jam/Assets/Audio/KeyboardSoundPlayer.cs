using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public List<AudioClip> KeyAudioClips;
    public List<AudioClip> SpacebarAudioClips;

    public void playKeyAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (KeyAudioClips.Count <= 0) return;
        AudioClip audioClip = KeyAudioClips[Random.Range(0, (KeyAudioClips.Count - 1))];
        audioSource.clip = audioClip;
        audioSource.pitch = (Random.Range(0.6f, .9f));
        audioSource.Play();
    }
    public void playSpacebarAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (SpacebarAudioClips.Count <= 0) return;
        AudioClip audioClip = SpacebarAudioClips[Random.Range(0, (SpacebarAudioClips.Count - 1))];
        audioSource.clip = audioClip;
        audioSource.pitch = (Random.Range(0.7f, 1.0f));
        audioSource.Play();
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
