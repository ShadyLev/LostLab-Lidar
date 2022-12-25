using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayParticleCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI displayText;
    private VFXGraphManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<VFXGraphManager>();
    }

    // Update is called once per frame
    void Update()
    {
        displayText.SetText("Particles: " + manager.GetCurrentParticleCount());
    }
}
