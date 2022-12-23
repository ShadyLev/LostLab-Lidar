using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// LIDAR scanner script. 
/// </summary>
/// <remarks>Written by Benedykt Cieslinski</remarks>
public class LIDARScanner : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] VFXGraphManager vfxManager;

    [Header("Point Types")]
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
    public float circleRadius;
    [Tooltip("Scanner range")]
    public float range;
    public float m_FullScanCharge;
    public float m_CurrentScanCharge;
    private bool startRecharge;
    private float minValue = 0.01f; // Minimum radius of the scan area
    private float maxValue = 1f; // Maxiumum radius of the scan area
    private float scrollValue = 0.005f; // How much the radius changes when scrolling
    private int laserScanScreenOffset = 50; // How much the radius changes when scrolling

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

    [SerializeField] private bool scanning;

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

        if (isScanning)
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
            scanning = true;

            isScanning = true; // We are scanning rn
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
        for (int i = 0; i < laserCount; i++)
        {
            Vector2 randomPointInCircle = GetRandomPointInsideCircle(circleRadius);

            Vector3 direction = playerCameraTransform.forward + (playerCameraTransform.right * randomPointInCircle.x) + (playerCameraTransform.up * randomPointInCircle.y);

            if (Physics.Raycast(playerCameraTransform.transform.position, direction, out rayHit, range, mask)) //whatIsEnemy
            {
                CheckHitObjectFromRaycast(rayHit);
                
                SetRandomColour();
                laserLineRenderer.SetPosition(0, muzzlePoint.position);
                laserLineRenderer.SetPosition(1, rayHit.point);
            }
        }
        vfxManager.SetCustomBufferData();
        //vfxManager.ApplyPositions();
    }

    void CheckHitObjectFromRaycast(RaycastHit hit)
    {
        foreach(PointType type in pointTypes)
        {
            if (hit.collider.CompareTag(type.TagName))
            {
                Vector4 pointColor = new Vector4(type.Color.r, type.Color.g, type.Color.b, type.Color.a);

                vfxManager.AddDataToBuffer(hit.point, pointColor, type.Lifetime);
            }
        }
    }

    Vector2 GetRandomPointInsideCircle(float circleRadius)
    {
        // Get a random point inside a circle
        float a = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        float r = UnityEngine.Random.Range(0, circleRadius);

        float x = Mathf.Sqrt(r * circleRadius) * Mathf.Cos(a);
        float y = Mathf.Sqrt(r * circleRadius) * Mathf.Sin(a);

        return new Vector2(x, y);
    }

    public void Scan(float deltaTime)
    {
        if (!scanning)
            return;
        
        
        if ((scanRate * deltaTime) + scanAngle >= verticalScanAngle)
        {
            scanAngle = -verticalScanAngle;
            scanning = false;

            isScanning = false; // No longer scanning
            startRecharge = true; // Start recharging
        }
        

        scanAngle += scanRate * deltaTime;

        NewScan();
    }

    public void CreateRayObjects(GameObject rayPrefab)
    {
        rays = new GameObject[numOfRays];
        scanAngle = verticalScanAngle;

        for (int i = 0; i < numOfRays; i++)
        {
            rays[i] = GameObject.Instantiate(rayPrefab, rayContainer);
        }
    }

    void NewScan()
    {
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < numOfRays; i++)
        {
            if (AdjustRayFromRaycast(rays[i].transform, horizontalScanAngle, scanAngle, Mathf.PI * UnityEngine.Random.Range(i / (float)numOfRays, i + 1 / (float)numOfRays), ref hit))
            {
                //if(vfxManager.CheckIfCanAddData())
                //    vfxManager.AddPositions(hit.point);
                //else
                //    vfxManager.CreateVFX();

                SetRandomColour();
                laserLineRenderer.SetPosition(0, muzzlePoint.position);
                laserLineRenderer.SetPosition(1, hit.point);
            }
        }
        //vfxManager.ApplyPositions();
    }

    private bool AdjustRayFromRaycast(Transform ray, float horizontalAngle, float verticalAngle, float horizontalRadians, ref RaycastHit hit)
    {
        bool successfulHit;

        // Orient the ray
        ray.localEulerAngles = new Vector3(verticalAngle, Mathf.Cos(horizontalRadians) * horizontalAngle, 0);

        successfulHit = Physics.Raycast(ray.position, ray.forward, out hit, 100000f, mask);

        return successfulHit;
    }


    /// <summary>
    /// Sets a random colour to the line renderer.
    /// </summary>
    void SetRandomColour()
    {
        float rnd = UnityEngine.Random.Range(0, 3);

        Color tmpCol = Color.red;

        if (rnd == 0)
            tmpCol = Color.red;
        else if (rnd == 1)
            tmpCol = Color.blue;
        else if (rnd == 2)
            tmpCol = Color.green;

        laserLineRenderer.startColor = tmpCol;
        laserLineRenderer.endColor = tmpCol;
    }
}
