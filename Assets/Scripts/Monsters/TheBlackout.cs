using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TheBlackout : MonoBehaviour
{
    // Reference to the player gameobject
    GameObject playerObject;

    // Reference to the VFX Manager script
    VFXGraphManager manager;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player").gameObject; // Get player object
        manager = playerObject.GetComponentInChildren<VFXGraphManager>(); // Get vfx manager script
    }

    /// <summary>
    /// Clears every VFX object in radius from self.
    /// </summary>
    /// <param name="radius">Radius of the clear.</param>
    public void BlackoutInArea(float radius)
    {
        int layerId = 8;
        int layerMask = 1 << layerId;
        Collider[] vfxNearbyArray = Physics.OverlapSphere(transform.position, radius, layerMask);

        foreach(Collider vfx in vfxNearbyArray)
        {
            if(Vector3.Distance(vfx.transform.position, transform.position) <= radius)
            {
                manager.DestroyOneFromVFXList(vfx.GetComponent<VisualEffect>());
            }
        }
    }

    /// <summary>
    /// Clears every VFX object in the scene.
    /// </summary>
    public void FullBlackout()
    {
        manager.DestroyVFXList();
    }
}
