using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudioController : MonoBehaviour
{
    private AIController controller;

    [Header("AudioSources")]
    [SerializeField] AudioSource defaultSource;

    [Header("Random clips array")]
    [SerializeField] AudioClip[] randomCreepyNoises;
    [SerializeField] float minDelayBetweenClips;
    [SerializeField] float maxDelayBetweenClips;

    [Header("Specified clips")]
    [SerializeField] AudioClip phase1DiscoveredClip;
    [SerializeField] AudioClip phase2DiscoveredClip;
    [SerializeField] AudioClip phase3KillClip;

    [SerializeField] bool isPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<AIController>();
    }

    // Update is called once per frame
    void Update()
    {
     
        

    }

    IEnumerator PlayRandomClip(AudioClip[] random)
    {
        isPlaying = true;

        int randomIndex = Random.Range(0, random.Length);

        AudioClip randomClip = random[randomIndex];

        defaultSource.PlayOneShot(randomClip, AudioManager.Instance.volumeSFX);

        yield return new WaitForSeconds(randomClip.length);

        isPlaying = false;
    }

    IEnumerator PlaySpecificClip(AudioClip clip)
    {
        isPlaying = true;
        defaultSource.PlayOneShot(clip, AudioManager.Instance.volumeSFX);

        yield return new WaitForSeconds(clip.length);

        isPlaying = false;
    }
}
