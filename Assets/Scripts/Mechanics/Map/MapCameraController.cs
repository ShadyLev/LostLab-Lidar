using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public Transform target;
    public Camera mainCamera;
    [Range(0.1f, 5f)]
    [Tooltip("How sensitive the mouse drag to camera rotation")]
    public float mouseRotateSpeed = 0.8f;
    [Range(0.01f, 100)]
    [Tooltip("How sensitive the touch drag to camera rotation")]
    public float touchRotateSpeed = 17.5f;
    [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
    public float slerpValue = 0.25f;


    private Vector2 swipeDirection; //swipe delta vector2
    private Quaternion cameraRot; // store the quaternion after the slerp operation
    private float distanceBetweenCameraAndTarget;

    private float minXRotAngle = -80; //min angle around x axis
    private float maxXRotAngle = 80; // max angle around x axis

    //Mouse rotation related
    private float rotX; // around x
    private float rotY; // around y

    // Start is called before the first frame update
    void Start()
    {
        distanceBetweenCameraAndTarget = Vector3.Distance(mainCamera.transform.position, target.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rotX += -Input.GetAxis("Mouse Y") * mouseRotateSpeed; // around X
            rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed;
        }

        if (rotX < minXRotAngle)
        {
            rotX = minXRotAngle;
        }
        else if (rotX > maxXRotAngle)
        {
            rotX = maxXRotAngle;
        }
    }

    private void LateUpdate()
    {

        Vector3 dir = new Vector3(0, 0, -distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target

        Quaternion newQ; // value equal to the delta change of our mouse or touch position

        newQ = Quaternion.Euler(rotX, rotY, 0); //We are setting the rotation around X, Y, Z axis respectively

        cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
        mainCamera.transform.position = target.position + cameraRot * dir;
        mainCamera.transform.LookAt(target.position);

    }

    public void SetCamPos()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        mainCamera.transform.position = new Vector3(0, 0, -distanceBetweenCameraAndTarget);
    }
}
