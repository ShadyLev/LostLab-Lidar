using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudioController : MonoBehaviour
{
    private AIController controller;

    [Header("AudioSources")]
    [SerializeField] AudioSource randomSource;
    [SerializeField] AudioSource oneShotSource;

    [Header("Random clips array")]
    [SerializeField] AudioClip[] randomCreepyNoises;
    [SerializeField] bool startTimer;
    [SerializeField] float minDelayBetweenClips;
    [SerializeField] float maxDelayBetweenClips;
    [SerializeField] float timer;

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
        //
        if (startTimer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                startTimer = false;
                StartPlayingRandomClip();
            }
        }
    }

    void StartPlayingRandomClip()
    {
        timer = Random.Range(minDelayBetweenClips, maxDelayBetweenClips);

        PlayRandomClip(randomCreepyNoises);
    }

    IEnumerator PlayRandomClip(AudioClip[] random)
    {
        isPlaying = true;

        int randomIndex = Random.Range(0, random.Length);

        AudioClip randomClip = random[randomIndex];

        randomSource.PlayOneShot(randomClip, AudioManager.Instance.volumeSFX);

        yield return new WaitForSeconds(randomClip.length);

        isPlaying = false;
        startTimer = true;
    }

    IEnumerator PlaySpecificClip(AudioClip clip)
    {
        isPlaying = true;
        oneShotSource.PlayOneShot(clip, AudioManager.Instance.volumeSFX);

        yield return new WaitForSeconds(clip.length);

        isPlaying = false;
    }
}
