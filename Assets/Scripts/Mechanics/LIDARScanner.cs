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

    [Header("Colour")]
    [Tooltip("Gradient of the colour based on distance from player.")]
    public Gradient gradient;

    [Tooltip("Distance at which the dot is far from player.")]
    public float farThreshhold;

    [Tooltip("Colour of the dot when hit enemy.")]
    public Color enemyHitColour;

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

    public bool Scanning
    {
        get => scanning;
        set
        {
            if (scanAngle < verticalScanAngle)
                return;

            scanning = value;
            scanAngle = scanning ? -verticalScanAngle : verticalScanAngle;
        }
    }

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

        InitializeRays(rayPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        Scan(Time.deltaTime);

        // Charge scanner
        if (startRecharge)
        {
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
            //laserLineRenderer.enabled = true; // Enable line renderer
            //StartCoroutine(LaserScan()); //LaserScan(800, 800);
            scanning = true;
        }
        //If player holds space perform a scan
        if (Input.GetKeyUp(KeyCode.Mouse1)) // Check if not shooting
        {
            //laserLineRenderer.enabled = true; // Enable line renderer
            //StartCoroutine(LaserScan()); //LaserScan(800, 800);
            scanning = false;
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
            // Get a random point inside a circle
            float a = Random.Range(0,2 * Mathf.PI);
            float r = Random.Range(0, circleRadius);

            float x = Mathf.Sqrt(r * circleRadius) * Mathf.Cos(a);
            float y = Mathf.Sqrt(r * circleRadius) * Mathf.Sin(a);

            // Add them to the ray direction
            //Vector3 direction = playerCameraTransform.transform.forward + new Vector3(x, y, 0); // this needs to change
            Vector3 direction = playerCameraTransform.forward + (playerCameraTransform.right * x) + (playerCameraTransform.up * y);

            if (Physics.Raycast(playerCameraTransform.transform.position, direction, out rayHit, range)) //whatIsEnemy
            {
                /*
                if (rayHit.collider.CompareTag("Enemy"))
                */
                if(vfxManager.CheckIfCanAddData())
                {
                    vfxManager.AddPositions(rayHit.point);
                    SetRandomColour();
                    laserLineRenderer.SetPosition(0, muzzlePoint.position);
                    laserLineRenderer.SetPosition(1, rayHit.point);
                }
                else
                {
                    vfxManager.CreateVFX();
                    break;
                }
            }
        }
        vfxManager.ApplyPositions();
    }

    /// <summary>
    /// Shoots rays in a shape of a rectangle one by one with a small delay between each ray.
    /// </summary>
    /// <param name="width">Width of the rectangle</param>
    /// <param name="height">Height of the rectangle</param>
    IEnumerator LaserScan()
    {
        isScanning = true; // We are scanning rn
        canScan = false; // Disable ability to scan again
        m_CurrentScanCharge = 0; // Set the charge time to 0

        Vector3 direction = playerCamera.transform.forward;
        Ray ray;

        for(int y = 0 + laserScanScreenOffset; y < Screen.height - laserScanScreenOffset; y += 10)
        {
            for (int x = 0 + laserScanScreenOffset; x < Screen.width - laserScanScreenOffset; x += 10)
            {
                ray = playerCamera.ScreenPointToRay(new Vector3(x, Screen.height - y, 0));

                Physics.Raycast(ray, out RaycastHit hit);

                if (hit.collider != null)
                {
                    if (vfxManager.CheckIfCanAddData())
                    {
                        vfxManager.AddPositions(hit.point);
                        SetRandomColour();
                        laserLineRenderer.SetPosition(0, muzzlePoint.position);
                        laserLineRenderer.SetPosition(1, hit.point);
                    }
                    else
                    {
                        vfxManager.CreateVFX();
                        break;
                    }
                }
                vfxManager.ApplyPositions();
            }
            yield return new WaitForSeconds(0.05f);
        }
        laserLineRenderer.enabled = false; // Disable line renderer
        isScanning = false; // No longer scanning
        startRecharge = true; // Start rechargin
        StopCoroutine(LaserScan());
    }

    public void Scan(float deltaTime)
    {
        if (!scanning)
            return;

        if ((scanRate * deltaTime) + scanAngle >= verticalScanAngle)
        {
            scanAngle = -scanAngle;
            scanning = false;
        }

        scanAngle += scanRate * deltaTime;
        NewScan();
        Debug.Log("scanning");
    }

    public void InitializeRays(GameObject rayPrefab)
    {
        rays = new GameObject[numOfRays];
        scanAngle = verticalScanAngle;

        for (int i = 0; i < numOfRays; i++)
        {
            rays[i] = GameObject.Instantiate(rayPrefab, rayContainer);
        }

        Scanning = false;
    }

    void NewScan()
    {
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < numOfRays; i++)
        {
            if (AdjustRayFromRaycast(rays[i].transform, horizontalScanAngle, scanAngle, Mathf.PI * Random.Range(i / (float)numOfRays, i + 1 / (float)numOfRays), ref hit))
            {
                vfxManager.AddPositions(hit.point);
                vfxManager.ApplyPositions();
                Debug.Log("hit");
            }
        }
    }

    private bool AdjustRayFromRaycast(Transform ray, float horizontalAngle, float verticalAngle, float horizontalRadians, ref RaycastHit hit)
    {
        bool successfulHit;

        // Orient the ray
        ray.localEulerAngles = new Vector3(verticalAngle, Mathf.Cos(horizontalRadians) * horizontalAngle, 0);

        int layerId = 7;
        int layerMask = 1 << layerId;

        successfulHit = Physics.Raycast(ray.position, ray.forward, out hit, 100000f);

        return successfulHit;
    }


    /// <summary>
    /// Sets a random colour to the line renderer.
    /// </summary>
    void SetRandomColour()
    {
        float rnd = Random.Range(0, 3);

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
