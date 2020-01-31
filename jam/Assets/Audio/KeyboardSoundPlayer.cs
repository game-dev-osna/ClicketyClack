using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardSoundPlayer : MonoBehaviour
{
    public AudioSource oneShotPrefab;
    public List<AudioClip> KeyAudioClips;
    public List<AudioClip> SpacebarAudioClips;

    public IEnumerator PlayKeyAudioasd()
    {
        if (KeyAudioClips.Count > 0)
        {

            AudioSource audioSource = Instantiate(oneShotPrefab);
            AudioClip audioClip = KeyAudioClips[Random.Range(0, (KeyAudioClips.Count - 1))];
            audioSource.clip = audioClip;
            audioSource.pitch = (Random.Range(0.8f, 1.0f));
            audioSource.Play();

            yield return new WaitForSeconds(audioClip.length);

            Destroy(audioSource.gameObject);
        }
    }
    public IEnumerator PlaySpacebarAudio()
    {
        if (SpacebarAudioClips.Count > 0)
        {
            AudioSource audioSource = Instantiate(oneShotPrefab);
            AudioClip audioClip = SpacebarAudioClips[Random.Range(0, (SpacebarAudioClips.Count - 1))];
            audioSource.clip = audioClip;
            audioSource.pitch = (Random.Range(0.8f, 1.0f));
            audioSource.Play();

            yield return new WaitForSeconds(audioClip.length);

            Destroy(audioSource.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PlaySpacebarAudio());
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(PlayKeyAudioasd());
        }
    }
}
