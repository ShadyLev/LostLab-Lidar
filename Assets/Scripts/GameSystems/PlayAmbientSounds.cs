using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAmbientSounds : MonoBehaviour
{
    [Header("Clip array")]
    [SerializeField] AudioClip[] clips;

    AudioSource source;

    // Index of the currently played sound
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayNote();
    }

    public void PlayNote()
    {
        // Play current sound
        source.PlayOneShot(clips[index], AudioManager.Instance.volumeMusic);

        // Increase the index, wrap around if reached end of array
        index = (index + 1) % clips.Length;
    }
}
