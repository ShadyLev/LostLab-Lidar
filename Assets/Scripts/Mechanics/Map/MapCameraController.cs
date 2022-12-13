using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;

    [Header("Camera Zoom Values")]
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float deltaZoom;
    [SerializeField] private float currentZoom;

    private Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        cam.transform.Translate(0, 0, -currentZoom);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        

        if (Input.GetMouseButton(0))
        {
            Vector3 direciton = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);

            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), direciton.y * 180);
            cam.transform.Rotate(new Vector3(0, 1, 0), -direciton.x * 180, Space.World);

            //cam.transform.Translate(0, 0, -currentZoom);
            ZoomMapCamera();

            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    void ZoomMapCamera()
    {
        // Adjust the zoom of the camera
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (currentZoom + deltaZoom >= maxZoom)
            {
                currentZoom = maxZoom;
                //cam.transform.Translate(0, 0, -currentZoom);
            }
            else
            {
                currentZoom += deltaZoom;
                //cam.transform.Translate(0, 0, -currentZoom);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (currentZoom - deltaZoom <= minZoom)
            {
                currentZoom = minZoom;
                //cam.transform.Translate(0, 0, currentZoom);
            }
            else
            {
                currentZoom -= deltaZoom;
                //cam.transform.Translate(0, 0, currentZoom);
            }
        }
        cam.transform.Translate(0, 0, -currentZoom);
    }
}
