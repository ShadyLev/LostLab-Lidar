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
    [SerializeField] public AudioClip phase1DiscoveredClip;
    [SerializeField] public AudioClip phase2DiscoveredClip;
    [SerializeField] public AudioClip phase3KillClip;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<AIController>();
        timer = Random.Range(minDelayBetweenClips, maxDelayBetweenClips);

        startTimer = true;
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
                Debug.Log("Playing random enemy clip");
            }
        }
    }

    void StartPlayingRandomClip()
    {
        timer = Random.Range(minDelayBetweenClips, maxDelayBetweenClips);

        StartCoroutine(PlayRandomClip(randomCreepyNoises));
    }

    IEnumerator PlayRandomClip(AudioClip[] random)
    {
        int randomIndex = Random.Range(0, random.Length);

        AudioClip randomClip = random[randomIndex];

        randomSource.PlayOneShot(randomClip, AudioManager.Instance.volumeSFX);

        yield return new WaitForSeconds(randomClip.length);

        startTimer = true;
    }

    public void PlaySpecificClip(AudioClip clip)
    {
        oneShotSource.PlayOneShot(clip, AudioManager.Instance.volumeSFX);

        Debug.Log("Playing specific sound");
    }
}
