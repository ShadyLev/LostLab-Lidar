using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudioController : MonoBehaviour
{
    [Header("AudioSources")]
    [SerializeField] AudioSource defaultSource;

    [Header("Random clips array")]
    [SerializeField] AudioClip[] randomCreepyNoises;

    [Header("Specified clips")]
    [SerializeField] AudioClip phase1DiscoveredClip;
    [SerializeField] AudioClip phase2DiscoveredClip;
    [SerializeField] AudioClip phase3KillClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayRandomClip(AudioClip[] random)
    {

    }

    void PlaySpecificClip()
    {

    }
}
