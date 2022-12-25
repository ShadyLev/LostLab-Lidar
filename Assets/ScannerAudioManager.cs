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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (scannerScript.GetBoolScanValues(ScanType.Normal))
        {
            Debug.Log(scannerScript.GetBoolScanValues(ScanType.Normal));
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
        audioSource.loop = true;

        audioSource.clip = normalScanClip;
        audioSource.volume = AudioManager.Instance.volumeSFX;

        audioSource.Play();
    }

    void PlayBigScanAudio()
    {
        audioSource.PlayOneShot(bigScanClip, AudioManager.Instance.volumeSFX);
    }
}
