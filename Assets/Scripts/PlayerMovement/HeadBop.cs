using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBop : MonoBehaviour
{
    [Header("Walking variables")]
    [SerializeField] float walkingBobbingSpeed = 14f;
    [SerializeField] float walkingBobbingAmount = 0.05f;

    // Reference to the playermovement script
    private PlayerMovement controller;

    float defaultPosY = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<PlayerMovement>();
        defaultPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isMoving)
        {
            //Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * walkingBobbingAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), transform.localPosition.z);
        }
    }
}
