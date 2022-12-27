using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerAudioManager : MonoBehaviour
{
    [SerializeField] private LIDARScanner scannerScript;
    [SerializeField] AudioSource _normalScanAudioSource;
    [SerializeField] AudioSource _bigScanAudioSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip normalScanClip;
    [SerializeField] AudioClip bigScanClip;

    bool playBigSoundOnce = false;

    private void Start()
    {
        SetUpNormalAudioSource();
    }

    // Update is called once per frame
    void Update()
    {
        if (!scannerScript.IsNormalScanning)
        {
            StopNormalScanAudio();
        }

        if (!scannerScript.IsBigScanning)
        {
            playBigSoundOnce = false;
        }

        if (scannerScript.IsNormalScanning && !_normalScanAudioSource.isPlaying)
        {
            PlayNormalScanAudio();
        }

        if (scannerScript.IsBigScanning && !playBigSoundOnce && !_bigScanAudioSource.isPlaying)
        {
            PlayBigScanAudio();
        }
    }

    void SetUpNormalAudioSource()
    {
        _normalScanAudioSource.loop = true;

        _normalScanAudioSource.clip = normalScanClip;
        _normalScanAudioSource.volume = AudioManager.Instance.volumeSFX;
    }

    void PlayNormalScanAudio()
    {
        _normalScanAudioSource.Play();
    }

    void StopNormalScanAudio()
    {
        _normalScanAudioSource.Stop();
    }

    void PlayBigScanAudio()
    {
        playBigSoundOnce = true;
        _bigScanAudioSource.PlayOneShot(bigScanClip, AudioManager.Instance.volumeSFX);
    }
}
