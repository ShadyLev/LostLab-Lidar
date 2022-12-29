using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [Header("Object references.")]
    [Tooltip("Object to rotate around.")]
    public Transform target;
    [Tooltip("Camera to rotate.")]
    public Camera mapCamera;

    [Header("Camera mouse controls.")]
    [Tooltip("How sensitive the mouse drag to camera rotation.")]
    [Range(0.1f, 5f)]
    [SerializeField] float mouseRotateSpeed = 0.8f;
    [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply.")]
    [Range(0,1)]
    [SerializeField] float slerpValue = 0.25f;

    [Tooltip("Min angle around x axis.")]
    [Range(-90,0)]
    [SerializeField] float minXRotAngle = -80;
    [Tooltip("Max angle around x axis.")]
    [Range(0, 90)]
    [SerializeField] float maxXRotAngle = 80;

    [Tooltip("How fast camera moves when dragged.")]
    [Range(0, 10)]
    [SerializeField] float dragSpeed = 2;

    [Header("Zoom values.")]
    [Tooltip("Max camera zoom out.")]
    [Range(0, 500)]
    [SerializeField] float maxZoom = 200f;
    [Tooltip("Min camera zoom in.")]
    [Range(0, 500)]
    [SerializeField] float minZoom = 50f;
    [Tooltip("How much the camera will zoom with each mouse scroll..")]
    [Range(0, 500)]
    [SerializeField] float zoomDelta = 10f;

    private Vector3 dragOrigin;

    //----HIDDEN VALUES----
    private float currentZoom = 20f;

    private GameObject targetRef;

    private Quaternion cameraRot; // store the quaternion after the slerp operation
    private float distanceBetweenCameraAndTarget; // distance

    //Mouse rotation related
    private float rotX; // around x
    private float rotY; // around y

    // Start is called before the first frame update
    void Start()
    {
        CreateRefTargetObject();

        distanceBetweenCameraAndTarget = Vector3.Distance(mapCamera.transform.position, targetRef.transform.position);
        distanceBetweenCameraAndTarget += currentZoom;
    }

    // Update is called once per frame
    void Update()
    {
        ZoomCamera();

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
        MoveCamera();

        RotateCamera();
    }

    private void OnEnable()
    {
        UpdateTargetRefPosition();
    }

    void RotateCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target

        Quaternion newQ; // value equal to the delta change of our mouse or touch position

        newQ = Quaternion.Euler(rotX, rotY, 0); //We are setting the rotation around X, Y, Z axis respectively

        cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
        mapCamera.transform.position = targetRef.transform.position + cameraRot * dir;
        mapCamera.transform.LookAt(targetRef.transform.position);
    }

    void MoveCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) 
            return;

        Vector3 pos = mapCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = (mapCamera.transform.right * -pos.x * dragSpeed) + (mapCamera.transform.up * -pos.y * dragSpeed);

        targetRef.transform.Translate(move, Space.World);
    }

    void SetCamPos()
    {
        if (mapCamera == null)
        {
            mapCamera = Camera.main;
        }
        mapCamera.transform.position = new Vector3(0, 0, -distanceBetweenCameraAndTarget);
    }

    void CreateRefTargetObject()
    {
        GameObject go = new GameObject("Camera Target Ref");

        go.transform.position = target.transform.position;

        targetRef = Instantiate(go, this.gameObject.transform);
    }

    void UpdateTargetRefPosition()
    {
        if (targetRef == null)
            return;

        targetRef.transform.position = target.transform.position;
    }

    void ZoomCamera()
    {
        // Adjust the radius of the scanner
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (currentZoom + zoomDelta > maxZoom)
            {
                currentZoom = maxZoom;
                distanceBetweenCameraAndTarget = currentZoom;
            }
            else
            {
                currentZoom += zoomDelta;
                distanceBetweenCameraAndTarget += currentZoom;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (currentZoom - zoomDelta < minZoom)
            {
                currentZoom = minZoom;
                distanceBetweenCameraAndTarget = currentZoom;
            }
            else
            {
                currentZoom -= zoomDelta;
                distanceBetweenCameraAndTarget -= currentZoom;
            }
        }
    }
}
