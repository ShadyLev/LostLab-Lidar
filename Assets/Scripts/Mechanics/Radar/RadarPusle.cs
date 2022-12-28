using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPusle : MonoBehaviour
{
    //---RADAR----
    [SerializeField] private Transform radarPingPrefab;
    [SerializeField] private Transform radarPingsContainer;
    [SerializeField] private LayerMask artefactLayerMask;
    [SerializeField] private Transform pulseSpriteTransform;
    [SerializeField] private float range;
    [SerializeField] private float rangeMax;
    [SerializeField] private float fadeRange;
    [SerializeField] private float rangeSpeed;

    //---AUDIO----
    [SerializeField] private AudioClip pingSound;
    [SerializeField] private float lowerPingSound;

    private AudioSource audioSource;

    private List<Collider> alreadyPingedColliders;

    private Color pulseColor;
    private SpriteRenderer pulseSpriteRenderer;


    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        pulseSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pulseColor = pulseSpriteRenderer.color;

        alreadyPingedColliders = new List<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        range += rangeSpeed * Time.deltaTime;

        if(range >= rangeMax)
        {
            range = 0f;
            alreadyPingedColliders.Clear();
        }

        pulseSpriteTransform.localScale = new Vector3(range, range);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range / 2f, artefactLayerMask);
        foreach (Collider col in hitColliders)
        {
            if (col == null)
                return;

            if (!alreadyPingedColliders.Contains(col))
            {
                alreadyPingedColliders.Add(col);

                if (col.gameObject.CompareTag("Artefact"))
                {
                    Vector3 newPingPosition = col.transform.position + new Vector3(0, 5, 0);
                    Transform radarPingTransform = Instantiate(radarPingPrefab, newPingPosition, Quaternion.Euler(90, 0, 0), radarPingsContainer);
                    RadarPing radarPing = radarPingTransform.GetComponent<RadarPing>();

                    radarPing.SetDisappearTimer(rangeMax / fadeRange);
                    radarPing.SetColour(Color.green);
                    audioSource.PlayOneShot(pingSound, AudioManager.Instance.volumeSFX - lowerPingSound);
                }
            }
        }


        // Fade of pulse 
        if(range > rangeMax - fadeRange)
        {
            pulseColor.a = Mathf.Lerp(0f, 1f, (rangeMax - range) / fadeRange);
        }
        else
        {
            pulseColor.a = 1f;
        }
        pulseSpriteRenderer.color = pulseColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, rangeMax/2);
    }
}
