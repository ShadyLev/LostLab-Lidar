using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    public float volumeGeneral = 1f;
    public float volumeSFX = 1f;
    public float volumeMusic = 1f;

    private static AudioSource audioSource;

    public AudioClip buttonHover;
    public AudioClip buttonClick;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        SetGeneralVolume();
    }

    public void ButtonClick()
    {
        audioSource.PlayOneShot(buttonClick, volumeSFX);
    }

    public void ButtonHover()
    {
        audioSource.PlayOneShot(buttonHover, volumeSFX);
    }

    public void SetGeneralVolume()
    {
        AudioListener.volume = volumeGeneral;
    }
}
