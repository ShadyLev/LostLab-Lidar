using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudioController : MonoBehaviour
{
    // AI controller ref
    private AIController controller;

    [Header("AudioSources")]
    [Tooltip("Audio source for random clips to play from.")]
    [SerializeField] AudioSource randomSource;
    [Tooltip("Audio source for specific clips to play from.")]
    [SerializeField] AudioSource oneShotSource;

    [Header("Random clips array")]
    [Tooltip("Array of clips to randomly play.")]
    [SerializeField] AudioClip[] randomCreepyNoises;
    [Tooltip("Bool to indicate if to play a clip and start the next timer.")]
    [SerializeField] bool startTimer;
    [Tooltip("Minimun delay between random clips.")]
    [SerializeField] float minDelayBetweenClips;
    [Tooltip("Maximum delay between random clips.")]
    [SerializeField] float maxDelayBetweenClips;
    float timer; // Timer 

    [Header("Specified clips")]
    [Tooltip("Clip that will play the 1st time enemy is discovered.")]
    [SerializeField] public AudioClip phase1DiscoveredClip;
    [Tooltip("Clip that will play the 2nd time enemy is discovered.")]
    [SerializeField] public AudioClip phase2DiscoveredClip;
    [Tooltip("Clip that will play when player is killed.")]
    [SerializeField] public AudioClip phase3KillClip;

    // Start is called before the first frame update
    void Start()
    {
        // Get AI controller
        controller = GetComponent<AIController>();

        // Set random timer
        timer = Random.Range(minDelayBetweenClips, maxDelayBetweenClips);

        // Start the timer
        startTimer = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Play clips with random delay between each one.
        if (startTimer)
        {
            timer -= Time.deltaTime;

            // If timer ends play a random clip.
            if (timer <= 0)
            {
                startTimer = false;
                StartPlayingRandomClip();
            }
        }
    }

    /// <summary>
    /// Sets a new random timer time and starts playing a new clip.
    /// </summary>
    void StartPlayingRandomClip()
    {
        timer = Random.Range(minDelayBetweenClips, maxDelayBetweenClips);

        StartCoroutine(PlayRandomClip(randomCreepyNoises));
    }

    /// <summary>
    /// Gets a random clip from an array and plays it.
    /// </summary>
    /// <param name="random">Array of random clips.</param>
    /// <returns>Coroutine wait time.</returns>
    IEnumerator PlayRandomClip(AudioClip[] random)
    {
        int randomIndex = Random.Range(0, random.Length);

        AudioClip randomClip = random[randomIndex];

        randomSource.PlayOneShot(randomClip, AudioManager.Instance.volumeSFX);

        yield return new WaitForSeconds(randomClip.length);

        startTimer = true;
    }

    /// <summary>
    /// Plays a clip with PlayOneShot method.
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySpecificClip(AudioClip clip)
    {
        oneShotSource.PlayOneShot(clip, AudioManager.Instance.volumeSFX);
    }
}
