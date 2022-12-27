using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerAudioManager : MonoBehaviour
{
    [SerializeField] private LIDARScanner scannerScript;
    private AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip normalScanClip;
    [SerializeField] AudioClip bigScanClip;

    public bool isPlaying;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!scannerScript.GetBoolScanValues(ScanType.Big))
            isPlaying = false;

        if (!scannerScript.GetBoolScanValues(ScanType.Normal))
        {
            isPlaying = false;
            audioSource.Stop();
        }

        if (isPlaying)
            return;

        if (scannerScript.GetBoolScanValues(ScanType.Normal))
        {
            if (!audioSource.isPlaying)
                PlayNormalScanAudio();
        }

        if (scannerScript.GetBoolScanValues(ScanType.Big))
        {
            Debug.Log(scannerScript.GetBoolScanValues(ScanType.Big));
            PlayBigScanAudio();
        }
    }

    void PlayNormalScanAudio()
    {
        isPlaying = true;
        audioSource.loop = true;

        audioSource.clip = normalScanClip;
        audioSource.volume = AudioManager.Instance.volumeSFX;

        audioSource.Play();
    }

    void PlayBigScanAudio()
    {
        isPlaying = true;
        audioSource.PlayOneShot(bigScanClip, AudioManager.Instance.volumeSFX);
    }
}
