using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the scanner audio.
/// </summary>
/// <remarks>Written by Benedykt Cieslinskis</remarks>
public class ScannerAudioManager : MonoBehaviour
{
    [Header("Script references.")]
    [Tooltip("Scanner script.")]
    [SerializeField] LIDARScanner scannerScript;

    [Header("Audio source components.")]
    [Tooltip("Audio source for playing normal scan sounds.")]
    [SerializeField] AudioSource _normalScanAudioSource;
    [Tooltip("Audio source for playing big scan sounds.")]
    [SerializeField] AudioSource _bigScanAudioSource;

    [Header("Audio Clips")]
    [Tooltip("Clip to play for normal scanning.")]
    [SerializeField] AudioClip normalScanClip;
    [Tooltip("Clip to play for big scan.")]
    [SerializeField] AudioClip bigScanClip;

    bool playBigSoundOnce = false;

    private void Start()
    {
        SetUpNormalAudioSource();
    }

    // Update is called once per frame
    void Update()
    {
        // If player is not using normal scan -> stop playing normal scan audio.
        if (!scannerScript.IsNormalScanning)
        {
            StopNormalScanAudio();
        }

        // If player is not using big scan -> stop playing big scan audio.
        if (!scannerScript.IsBigScanning)
        {
            playBigSoundOnce = false;
        }

        // If player is normal scanning && the audio source is not playing -> start playing normal scan audio.
        if (scannerScript.IsNormalScanning && !_normalScanAudioSource.isPlaying)
        {
            PlayNormalScanAudio();
        }

        // If player is big scanning && we're not already playing -> start audios.
        if (scannerScript.IsBigScanning && !playBigSoundOnce && !_bigScanAudioSource.isPlaying)
        {
            PlayBigScanAudio();
        }
    }

    /// <summary>
    /// Sets the audio source values for later play.
    /// </summary>
    void SetUpNormalAudioSource()
    {
        _normalScanAudioSource.loop = true;

        _normalScanAudioSource.clip = normalScanClip;
        _normalScanAudioSource.volume = AudioManager.Instance.volumeSFX;
    }

    /// <summary>
    /// Play audio source clip.
    /// </summary>
    void PlayNormalScanAudio()
    {
        _normalScanAudioSource.Play();
    }

    /// <summary>
    /// Stop audio source clip.
    /// </summary>
    void StopNormalScanAudio()
    {
        _normalScanAudioSource.Stop();
    }

    /// <summary>
    /// Play one shot big scan clip.
    /// </summary>
    void PlayBigScanAudio()
    {
        playBigSoundOnce = true;
        _bigScanAudioSource.PlayOneShot(bigScanClip, AudioManager.Instance.volumeSFX);
    }
}
