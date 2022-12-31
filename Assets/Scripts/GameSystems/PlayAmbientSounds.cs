using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAmbientSounds : MonoBehaviour
{
    [Header("Clip array")]
    [SerializeField] AudioClip[] clips;

    AudioSource source;
    float currentVolume;

    // Index of the currently played sound
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        currentVolume = AudioManager.Instance.volumeMusic;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustVolume();

        if (source.isPlaying)
            return;

        PlayNote();
    }

    public void PlayNote()
    {
        Debug.Log("Player random");

        // Play current sound
        source.clip = clips[index];
        source.volume = AudioManager.Instance.volumeMusic;
        source.Play();

        // Increase the index, wrap around if reached end of array
        index = (index + 1) % clips.Length;
    }

    void AdjustVolume()
    {
        if(currentVolume != AudioManager.Instance.volumeMusic)
        {
            currentVolume = AudioManager.Instance.volumeMusic;
            source.volume = AudioManager.Instance.volumeMusic;
        }
    }
}
