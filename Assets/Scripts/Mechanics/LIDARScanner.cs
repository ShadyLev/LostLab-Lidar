using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum ScanType
{
    Normal,
    Big
}

/// <summary>
/// LIDAR scanner script. 
/// </summary>
/// <remarks>Written by Benedykt Cieslinski</remarks>
public class LIDARScanner : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] VFXGraphManager vfxManager;

    [NonReorderable]
    [SerializeField] PointType[] pointTypes;

    [Header("Transforms")]
    [Tooltip("Muzzle point of the scanner. From here the line renderer will start.")]
    public Transform muzzlePoint;
    [Tooltip("Reference to the player camera transform.")]
    public Transform playerCameraTransform;
    [Tooltip("Reference to the player camera.")]
    public Camera playerCamera;

    [Header("LIDAR values")]
    [Tooltip("Radius of the scan area.")]
    [SerializeField] private float circleRadius;
    [Tooltip("Scanner range.")]
    [SerializeField] private float range;
    [Tooltip("How long the big scan will recharge.")]
    [SerializeField] public float m_FullScanCharge;

    public float m_CurrentScanCharge; // current scan charge value
    private bool startRecharge; // Start recharge bool
    private float minValue = 0.01f; // Minimum radius of the scan area
    private float maxValue = 1f; // Maxiumum radius of the scan area
    private float scrollValue = 0.005f; // How much the radius changes when scrolling

    RaycastHit rayHit; // Reference to the raycast hit

    [Header("Line Renderer")]
    [Tooltip("Width of the line renderer.")]
    public float laserWidth = 0.1f;
    private LineRenderer laserLineRenderer;

    private bool isScanning = false;
    public bool canScan = true;

    [SerializeField] private GameObject rayPrefab;
    [SerializeField] private Transform rayContainer;
    [SerializeField] private int numOfRays;
    [SerializeField] private float verticalScanAngle;
    [SerializeField] private float horizontalScanAngle;
    [SerializeField] private float scanRate;

    private GameObject[] rays;
    [SerializeField] private float scanAngle;

    [SerializeField] private bool m_bBigCcanning;
    [SerializeField] private bool m_bNormalScanning;

    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        // Set line renderer values
        laserLineRenderer = GetComponent<LineRenderer>();
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
        laserLineRenderer.startColor = Color.red;
        laserLineRenderer.endColor = Color.red;

        CreateRayObjects(rayPrefab);

        m_CurrentScanCharge = m_FullScanCharge;
        scanAngle = -verticalScanAngle;
    }

    // Update is called once per frame
    void Update()
    {
        Scan(Time.deltaTime);

        // Charge scanner
        if (startRecharge)
        {
            laserLineRenderer.enabled = false; // Enable line renderer

            if (m_CurrentScanCharge >= m_FullScanCharge)
            {
                startRecharge = false;
                canScan = true;
            }

            m_CurrentScanCharge += Time.deltaTime;
        }

        // Get player input
        GetInput();
    }

    public bool GetBoolScanValues(ScanType type)
    {
        bool scanBool = false;

        switch (type)
        {
            case ScanType.Normal:
                scanBool = m_bNormalScanning;
                break;
            case ScanType.Big:
                scanBool = m_bBigCcanning;
                break;
        }

        return scanBool;
    }

    /// <summary>
    /// Gets the player inputs
    /// </summary>
    void GetInput()
    {
        // Adjust the radius of the scanner
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (circleRadius + scrollValue > maxValue)
                circleRadius = maxValue;
            else
                circleRadius += scrollValue;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (circleRadius - scrollValue < minValue)
                circleRadius = minValue;
            else
                circleRadius -= scrollValue;
        }

        if (m_bBigCcanning)
            return;

        // If player holds M1 shoot laser
        if (Input.GetKey(KeyCode.Mouse0)) // check if not scanning
        {
            laserLineRenderer.enabled = true; // Enable line renderer
            ShootLaser(15); // Shoot laser
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            laserLineRenderer.enabled = false; // Disable line renderer
        }

        //If player holds space perform a scan
        if (Input.GetKey(KeyCode.Mouse1) && canScan) // Check if not shooting
        {
            laserLineRenderer.enabled = true; // Enable line renderer
            m_bBigCcanning = true;

            canScan = false; // Disable ability to scan again
            m_CurrentScanCharge = 0; // Set the charge time to 0
        }
    }

    /// <summary>
    /// Shoots a ray with a random spread in a shape of a circle,
    /// and draws a line renderer between the hit point and the origin point.
    /// </summary>
    void ShootLaser(int laserCount)
    {
        m_bNormalScanning = true;

        for (int i = 0; i < laserCount; i++)
        {
            // Get a random point inside a circle
            Vector2 randomPointInCircle = GetRandomPointInsideCircle(circleRadius);

            // Calculate raycast direction
            Vector3 direction = playerCameraTransform.forward + (playerCameraTransform.right * randomPointInCircle.x) + (playerCameraTransform.up * randomPointInCircle.y);

            if (Physics.Raycast(playerCameraTransform.transform.position, direction, out rayHit, range, mask))
            {
                // Apply positions
                CheckHitColliderOnPointType(rayHit);
                
                /// Set line renderer variables
                SetRandomColour(); // Random colour
                laserLineRenderer.SetPosition(0, muzzlePoint.position); // Start point
                laserLineRenderer.SetPosition(1, rayHit.point); // End point
            }
        }
        vfxManager.SetCustomBufferData();
        m_bNormalScanning = false;
    }

    /// <summary>
    /// Checks if RaycastHit hit a object with a particular tag and assigns buffer data based on the tag.
    /// </summary>
    /// <param name="hit">RaycastHit variable</param>
    void CheckHitColliderOnPointType(RaycastHit hit)
    {
        // Go through each added point type.
        foreach(PointType type in pointTypes)
        {
            if (hit.collider.CompareTag(type.TagName)) // Compare tag
            {
                // Change Color type to Vector4 (Graphics Buffer does not support Color data type)
                Vector4 pointColor = new Vector4(type.Color.r, type.Color.g, type.Color.b, type.Color.a);

                // Convert bool into int (Graphics Buffer does not support Bool data type)
                int isGrad = type.useDefaultGradient ? 1 : 0;

                // Add data to buffer
                vfxManager.AddDataToBuffer(hit.point, pointColor, isGrad, type.Size);
            }
        }
    }

    /// <summary>
    /// Calculates a random point inside a circle.
    /// </summary>
    /// <param name="circleRadius">Radius of the circle</param>
    /// <returns>Vector2 coordinates of a point inside a circle.</returns>
    Vector2 GetRandomPointInsideCircle(float circleRadius)
    {
        // Get a random point inside a circle
        float a = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        float r = UnityEngine.Random.Range(0, circleRadius);

        float x = Mathf.Sqrt(r * circleRadius) * Mathf.Cos(a);
        float y = Mathf.Sqrt(r * circleRadius) * Mathf.Sin(a);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Creates gameobjects to shoot rays from.
    /// </summary>
    /// <param name="rayPrefab">Prefab of the gameobject.</param>
    private void CreateRayObjects(GameObject rayPrefab)
    {
        rays = new GameObject[numOfRays];
        scanAngle = verticalScanAngle;

        for (int i = 0; i < numOfRays; i++)
        {
            rays[i] = GameObject.Instantiate(rayPrefab, rayContainer);
        }
    }

    /// <summary>
    /// Calculates scan angle based on deltatime and calls Scan function.
    /// </summary>
    /// <param name="deltaTime"></param>
    private void Scan(float deltaTime)
    {
        // If not scanning return
        if (!m_bBigCcanning)
            return;

        // If scan angle is above max limit stop scanning
        if ((scanRate * deltaTime) + scanAngle >= verticalScanAngle)
        {
            scanAngle = -verticalScanAngle; // Reset scan angle
            m_bBigCcanning = false;

            startRecharge = true; // Start recharging
        }
        
        // Increase scan angle
        scanAngle += scanRate * deltaTime;

        // Shoot rays
        ShootRays();
    }

    /// <summary>
    /// Shoots rays in rows and sets hit points to graphics buffer.
    /// </summary>
    private void ShootRays()
    {
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < numOfRays; i++)
        {
            if (RotateRayObjectsToAngle(rays[i].transform, horizontalScanAngle, scanAngle, Mathf.PI * UnityEngine.Random.Range(i / (float)numOfRays, i + 1 / (float)numOfRays), ref hit))
            {
                CheckHitColliderOnPointType(hit);

                SetRandomColour();
                laserLineRenderer.SetPosition(0, muzzlePoint.position);
                laserLineRenderer.SetPosition(1, hit.point);
            }
        }
        vfxManager.SetCustomBufferData();
    }

    /// <summary>
    /// Calculates the angle of the ray object and shoots a ray forward.
    /// </summary>
    /// <param name="ray">Transform of an object to shoot the ray from</param>
    /// <param name="horizontalAngle">Max Horizontal Angle to shoot ray at.</param>
    /// <param name="verticalAngle">Max Vertical Angle to shoot ray at.</param>
    /// <param name="horizontalRadians">Radian angle between each ray.</param>
    /// <param name="hit">Raycast hit reference.</param>
    /// <returns>True or false based on if raycast hit an object.</returns>
    private bool RotateRayObjectsToAngle(Transform ray, float horizontalAngle, float verticalAngle, float horizontalRadians, ref RaycastHit hit)
    {
        bool successfulHit;

        // Rotate the ray to target
        ray.localEulerAngles = new Vector3(verticalAngle, Mathf.Cos(horizontalRadians) * horizontalAngle, 0);

        // Shoot raycast in direction
        successfulHit = Physics.Raycast(ray.position, ray.forward, out hit, 100000f, mask);

        // Return bool #
        return successfulHit;
    }


    /// <summary>
    /// Sets a random colour to the line renderer.
    /// </summary>
    void SetRandomColour()
    {
        float rnd = UnityEngine.Random.Range(0, 3);

        Color tmpCol;

        switch (rnd)
        {
            default:
                tmpCol = Color.yellow;
                break;
            case 0:
                tmpCol = Color.red;
                break;
            case 1:
                tmpCol = Color.blue;
                break;
            case 2:
                tmpCol = Color.green;
                break;
        }

        laserLineRenderer.startColor = tmpCol;
        laserLineRenderer.endColor = tmpCol;
    }
}
