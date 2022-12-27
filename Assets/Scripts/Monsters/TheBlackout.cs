using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TheBlackout : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] VFXGraphManager manager;
    [SerializeField] float radius;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player").gameObject;
        manager = playerObject.GetComponentInChildren<VFXGraphManager>();
    }

    public void BlackoutInArea(float radius)
    {
        int layerId = 8;
        int layerMask = 1 << layerId;
        Collider[] vfxNearbyArray = Physics.OverlapSphere(playerObject.transform.position, radius, layerMask);

        foreach(Collider vfx in vfxNearbyArray)
        {
            if(Vector3.Distance(vfx.transform.position, playerObject.transform.position) <= radius)
            {
                //manager.DestroyOneFromVFXList(vfx.GetComponent<VisualEffect>());
                Destroy(vfx.gameObject);
            }
        }
    }

    public void FullBlackout()
    {
        manager.DestroyVFXList();
    }
}
