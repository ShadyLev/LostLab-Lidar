using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera values")]
    [Tooltip("Sensitivity on the X axis")]
    public float sensX; // Sensitivity on the X axis
    [Tooltip("Sensitivity on the Y axis")]
    public float sensY; // Sensitivity on the Y axis

    [Header("Object references")]
    [Tooltip("Player orientation object")]
    public Transform player;

    // Private variables
    float xRotation; // X axis rotation of the camera
    float yRotation; // Y axis rotation of the camera

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        // Get the mouse inputs
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamping the rotation so player cannot break their neck

        // Rotate the camera and player rotation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        player.rotation = Quaternion.Euler(0, yRotation, 0);

    }
}
