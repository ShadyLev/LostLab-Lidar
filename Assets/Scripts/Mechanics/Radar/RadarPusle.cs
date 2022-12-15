using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPusle : MonoBehaviour
{
    [SerializeField] private Transform radarPingPrefab;

    [SerializeField] private Transform radarPingsContainer;

    [SerializeField] private LayerMask artefactLayerMask;

    [SerializeField] private Transform pulseSpriteTransform;
    [SerializeField] private float range;
    [SerializeField] private float rangeMax;
    [SerializeField] private float fadeRange;
    [SerializeField] private float rangeSpeed;
    private List<Collider> alreadyPingedColliders;

    private Color pulseColor;
    private SpriteRenderer pulseSpriteRenderer;


    // Start is called before the first frame update
    void Awake()
    {
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

        RaycastHit[] hitsArray = Physics.SphereCastAll(transform.position, range / 2f, transform.forward, artefactLayerMask);
        foreach (RaycastHit rayHit in hitsArray) {
            if (rayHit.collider != null)
            {
                if (!alreadyPingedColliders.Contains(rayHit.collider))
                {
                    alreadyPingedColliders.Add(rayHit.collider);

                    //Debug.Log("Hit object " + rayHit.collider.name);
                    if (rayHit.collider.CompareTag("Artefact"))
                    {
                        Vector3 newPingPosition = rayHit.collider.transform.position + new Vector3(0, 5, 0);
                        Transform radarPingTransform = Instantiate(radarPingPrefab, newPingPosition, Quaternion.Euler(90,0,0), radarPingsContainer);
                        RadarPing radarPing = radarPingTransform.GetComponent<RadarPing>();

                        radarPing.SetDisappearTimer(rangeMax / fadeRange);
                        radarPing.SetColour(Color.green);
                    }
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
}
