using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Enums for different scanner scan modes.
/// </summary>
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
    #region Variables

    [Header("Particle Systems")]
    [Space(2)]
    [SerializeField] VFXGraphManager vfxManager;

    [Header("Particle Color Options")]
    [Space(2)]
    [Tooltip("Default gradient for particles to use based on the distance from player.")]
    [SerializeField] Gradient defaultParticleGradient;
    [Tooltip("Max distance from player at which gradient will end.")]
    [SerializeField] float gradientMaxDistance;

    [NonReorderable]
    [SerializeField] PointType[] pointTypes;

    [Header("Transforms")]
    [Tooltip("Muzzle point of the scanner. From here the line renderer will start.")]
    public Transform muzzlePoint;
    [Tooltip("Reference to the player camera transform.")]
    public Transform playerCameraTransform;

    [Header("General Scanner Values")]
    [Tooltip("Layermasks for the raycasts to hit.")]
    [SerializeField] LayerMask mask;
    [Tooltip("Key that when pressed will perform a normal scan.")]
    [SerializeField] KeyCode m_normalScanKey;
    [Tooltip("Key that when pressed will perform a big scan.")]
    [SerializeField] KeyCode m_bigScanKey;
    [Tooltip("How much the radius changes when scrolling")]
    [Range(0,1)]
    [SerializeField] private float m_scrollValue; 

    [Header("Normal scan values")]
    [Tooltip("Number of rays shot per frame.")]
    [Range(0, 50)]
    [SerializeField] private int m_numberOfNormalScanRays;
    [Space(2)]
    [Tooltip("Minimum radius of the scan area.")]
    [Range(0, 5)]
    [SerializeField] private float m_minCircleRadius;
    [Tooltip("Maxiumum radius of the scan area.")]
    [Range(0, 5)]
    [SerializeField] private float m_maxCircleRadius;
    [Tooltip("Default radius of the scan area.")]
    [Range(0, 5)]
    [SerializeField] private float m_defaultCircleRadius;
    [Tooltip("Scanner range.")]
    [Range(0, 1000)]
    [SerializeField] private float m_range;

    [Header("Big scan values")]
    [Space(2)]
    [Tooltip("GameObject that will hold transforms for scan reference.")]
    [SerializeField] private Transform rayContainer;
    [Tooltip("Number of rays per row.")]
    [Range(0, 200)]
    [SerializeField] private int m_numberOfBigScanRays;
    [Tooltip("The vertical angle of scan.")]
    [Range(0, 100)]
    [SerializeField] private float m_verticalScanAngle;
    [Tooltip("The horizontal angle of scan.")]
    [Range(0, 100)]
    [SerializeField] private float m_horizontalScanAngle;
    [Tooltip("Speed of the scan.")]
    [Range(0, 100)]
    [SerializeField] private float m_scanRate;
    [Tooltip("Scan recharge time.")]
    [Range(0, 25)]
    [SerializeField] public float m_FullScanCharge;

    [Header("Line Renderer")]
    [Space(2)]
    [Tooltip("Width of the line renderer.")]
    [Range(0,1)]
    [SerializeField] private float m_laserWidth = 0.1f;
    [Tooltip("Check true if you want the line renderer to only use 1 color for each ray.")]
    [SerializeField] private bool m_useOneColor;
    [Tooltip("Single color for rays.")]
    [SerializeField] private Color m_singleRayColor;
    [Tooltip("Array of colors that will be chosen at random when shooting rays.")]
    [SerializeField] private Color[] m_randomArrayOfColors;

    //----HIDDEN VARIABLES----

    // Normal scan hidden variables
    RaycastHit rayHit; // Reference to the raycast hit for normal scanning
    private float m_circleRadius; // Current scan circle radius

    // Line renderer hidden variables
    private LineRenderer laserLineRenderer; // Reference to the line renderer

    // Big scan hidden variables
    private bool canScan = true; // Can player perform big scan bool
    private GameObject[] rays; // Array of reference ray objects for scan
    private bool startRecharge; // Start scan recharge bool
    private float scanAngle; // Current scan angle
    private float m_CurrentScanCharge; // current scan charge value

    // Is player scanning variables
    private bool m_isBigScanning = false;
    private bool m_isNormalScanning = false;

    #endregion

    #region Awake, Start & Update

    // Start is called before the first frame update
    void Awake()
    {
        // Set initial variable values
        m_circleRadius = m_defaultCircleRadius; // Starting circle radius = default circle radius
        m_CurrentScanCharge = m_FullScanCharge; // Current scan charge = full scan charge

        // Set the line renderer data
        SetLineRendererData();

        // Apply gradient and max distance to vfx graph manager
        //vfxManager.SetDefaultGradientData(defaultParticleGradient, gradientMaxDistance);
        vfxManager.DefaultParticleGradient = defaultParticleGradient;
        vfxManager.GradientMaxDistance = gradientMaxDistance;

        // Instantiate ray objects for big scan reference 
        CreateRayObjects();
    }

    // Update is called once per frame
    void Update()
    {
        // Call big scan method each frame
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

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Based on a scan type it return a bool that indicates if that scan is being performed.
    /// </summary>
    /// <param name="type">Scanner scan type.</param>
    /// <returns>Bool -> True if is scanning of type.</returns>
    public bool GetBoolScanValues(ScanType type)
    {
        bool scanBool = false;

        switch (type)
        {
            case ScanType.Normal:
                scanBool = m_isNormalScanning;
                break;
            case ScanType.Big:
                scanBool = m_isBigScanning;
                break;
        }

        return scanBool;
    }

    /// <summary>
    /// Gets the private value of scanner's current scan charge.
    /// </summary>
    /// <returns>Float of m_CurrentScanCharge.</returns>
    public float GetCurrentScanCharge()
    {
        return m_CurrentScanCharge;
    }

    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Gets the player inputs
    /// </summary>
    private void GetInput()
    {
        // Adjust the radius of the scanner
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (m_circleRadius + m_scrollValue > m_maxCircleRadius)
                m_circleRadius = m_maxCircleRadius;
            else
                m_circleRadius += m_scrollValue;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (m_circleRadius - m_scrollValue < m_minCircleRadius)
                m_circleRadius = m_minCircleRadius;
            else
                m_circleRadius -= m_scrollValue;
        }

        // If big scan is active do not progress forward
        if (m_isBigScanning)
            return;

        // Check if normal key is pressed and perform scan
        if (Input.GetKey(m_normalScanKey))
        {
            m_isNormalScanning = true; // Set scanning to true
            laserLineRenderer.enabled = true; // Enable line renderer
            ShootLaser(m_numberOfNormalScanRays); // Shoot laser
        }
        else if (Input.GetKeyUp(m_normalScanKey))
        {
            laserLineRenderer.enabled = false; // Disable line renderer
            m_isNormalScanning = false; // Set scanning to false
        }

        //Check if big scan key is pressed and perform scan
        if (Input.GetKey(m_bigScanKey) && canScan)
        {
            laserLineRenderer.enabled = true; // Enable line renderer
            m_isBigScanning = true; // Set scanning to true

            canScan = false; // Disable ability to scan again
            m_CurrentScanCharge = 0; // Set the charge time to 0
        }
    }

    /// <summary>
    /// Checks if RaycastHit hit a object with a particular tag and assigns buffer data based on the tag.
    /// </summary>
    /// <param name="hit">RaycastHit variable</param>
    private void CheckHitColliderOnPointType(RaycastHit hit)
    {
        // Go through each added point type.
        foreach (PointType type in pointTypes)
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

    #region Normal scan

    /// <summary>
    /// Shoots a ray with a random spread in a shape of a circle,
    /// and draws a line renderer between the hit point and the origin point.
    /// </summary>
    private void ShootLaser(int laserCount)
    {
        for (int i = 0; i < laserCount; i++)
        {
            // Get a random point inside a circle
            Vector2 randomPointInCircle = GetRandomPointInsideCircle(m_circleRadius);

            // Calculate raycast direction
            Vector3 direction = playerCameraTransform.forward + (playerCameraTransform.right * randomPointInCircle.x) + (playerCameraTransform.up * randomPointInCircle.y);

            if (Physics.Raycast(playerCameraTransform.transform.position, direction, out rayHit, m_range, mask))
            {
                // Apply positions
                CheckHitColliderOnPointType(rayHit);

                // Set line renderer variables
                UpdateLineRendererData(muzzlePoint.position, rayHit.point);
            }
        }

        // Set the data to the buffer
        vfxManager.SetCustomBufferData();
    }

    /// <summary>
    /// Calculates a random point inside a circle.
    /// </summary>
    /// <param name="circleRadius">Radius of the circle</param>
    /// <returns>Vector2 coordinates of a point inside a circle.</returns>
    private Vector2 GetRandomPointInsideCircle(float circleRadius)
    {
        float a = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        float r = UnityEngine.Random.Range(0, circleRadius);

        float x = Mathf.Sqrt(r * circleRadius) * Mathf.Cos(a);
        float y = Mathf.Sqrt(r * circleRadius) * Mathf.Sin(a);

        return new Vector2(x, y);
    }

    #endregion

    #region Big Scan 

    /// <summary>
    /// Creates gameobjects to shoot rays from.
    /// </summary>
    private void CreateRayObjects()
    {
        GameObject tmp = new GameObject("RayTransformRef");
        rays = new GameObject[m_numberOfBigScanRays];
        scanAngle = -m_verticalScanAngle;

        for (int i = 0; i < m_numberOfBigScanRays; i++)
        {
            rays[i] = GameObject.Instantiate(tmp, rayContainer);
        }
    }

    /// <summary>
    /// Calculates scan angle based on deltatime and calls Scan function.
    /// </summary>
    /// <param name="deltaTime"></param>
    private void Scan(float deltaTime)
    {
        // If not scanning return
        if (!m_isBigScanning)
            return;

        // If scan angle is above max limit stop scanning
        if ((m_scanRate * deltaTime) + scanAngle >= m_verticalScanAngle)
        {
            scanAngle = -m_verticalScanAngle; // Reset scan angle
            m_isBigScanning = false;

            startRecharge = true; // Start recharging
        }
        
        // Increase scan angle
        scanAngle += m_scanRate * deltaTime;

        // Shoot rays
        ShootRays();
    }

    /// <summary>
    /// Shoots rays in rows and sets hit points to graphics buffer.
    /// </summary>
    private void ShootRays()
    {
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < m_numberOfBigScanRays; i++)
        {
            if (RotateRayObjectsToAngle(rays[i].transform, m_horizontalScanAngle, scanAngle, Mathf.PI * UnityEngine.Random.Range(i / (float)m_numberOfBigScanRays, i + 1 / (float)m_numberOfBigScanRays), ref hit))
            {
                CheckHitColliderOnPointType(hit);

                UpdateLineRendererData(muzzlePoint.position, hit.point);
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

        // Return bool
        return successfulHit;
    }

    #endregion

    #region Line Renderer

    /// <summary>
    /// Sets up the default line renderer values.
    /// </summary>
    private void SetLineRendererData()
    {
        // Get line renderer component
        laserLineRenderer = GetComponent<LineRenderer>();

        // Set initial line positions
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);

        // Set line widths
        laserLineRenderer.startWidth = m_laserWidth;
        laserLineRenderer.endWidth = m_laserWidth;

        // Set initial line colors
        laserLineRenderer.startColor = Color.red;
        laserLineRenderer.endColor = Color.red;
    }

    private void UpdateLineRendererData(Vector3 startPos, Vector3 endPos)
    {
        if (m_useOneColor)
        {
            laserLineRenderer.startColor = m_singleRayColor;
            laserLineRenderer.endColor = m_singleRayColor;
        }else if (!m_useOneColor)
        {
            SetRandomColour(m_randomArrayOfColors);
        }

        laserLineRenderer.SetPosition(0, startPos);
        laserLineRenderer.SetPosition(1, endPos);
    }

    /// <summary>
    /// Sets a random colour to the line renderer.
    /// </summary>
    private void SetRandomColour(Color[] colors)
    {
        int rnd = UnityEngine.Random.Range(0, colors.Length);

        laserLineRenderer.startColor = colors[rnd];
        laserLineRenderer.endColor = colors[rnd];
    }

    #endregion

    #endregion
}
