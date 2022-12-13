using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TheBlackout : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] float radius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Blackout()
    {
        Collider[] vfxNearbyArray = Physics.OverlapSphere(playerObject.transform.position, radius);

        foreach(Collider vfx in vfxNearbyArray)
        {
            
        }
    }
}
