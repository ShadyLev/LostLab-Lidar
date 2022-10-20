using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move values")]
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed = 8.0f;
    [SerializeField] float gravity = 20.0f;

    CharacterController charController;
    Vector3 moveDirection;

    public bool isMoving;

    private Vector3 lastPosition = Vector3.zero;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if(charController.velocity.magnitude > 0.5)
            isMoving = true;
        else
            isMoving = false;

        if (charController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;
            moveDirection = this.transform.TransformDirection(moveDirection);

            
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
            
        }

        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        charController.Move(moveDirection * Time.deltaTime);
    }
}
