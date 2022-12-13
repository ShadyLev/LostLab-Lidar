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
    private const string TEXTURE_NAME = "PositionsTexture"; // Reference to VFX Graph variable
    private const string RESOLUTION_PARAMETER_NAME = "Resolution"; // Reference to VFX Graph variable

    private List<Vector3> positionsList = new List<Vector3>(); // List holding all positions of hit points 
    private List<VisualEffect> vfxList = new List<VisualEffect>(); // List of VFX Graphs
    private VisualEffect currentVFX; // Current used VFX Graph
    private Texture2D texture; // Texture holding DATA info
    private Color[] positions; // Positions encoded in a Color array
    [SerializeField]private int particleAmount; // Current amount of particles
    private bool createNewVFX = false; // Create new vfx bool

    [SerializeField] VisualEffect vfxPrefab; // VFX Graph prefab
    [SerializeField] GameObject vfxContainer; // Gameobject holding all VFX Graph prefabs in scene

    private int resolution = 100; // Resolution of the texture2d holding data points

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

        createNewVFX = true;

        CreateVFX();
        ApplyPositions();
    }

    // Update is called once per frame
    void Update()
    {
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
            laserLineRenderer.enabled = true; // Enable line renderer
            StartCoroutine(LaserScan()); //LaserScan(800, 800);
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
                if(positionsList.Count < resolution * resolution)
                {
                    positionsList.Add(rayHit.point);
                    particleAmount++;
                    SetRandomColour();
                    laserLineRenderer.SetPosition(0, muzzlePoint.position);
                    laserLineRenderer.SetPosition(1, rayHit.point);
                }
                else
                {
                    createNewVFX = true;
                    CreateVFX();
                    break;
                }
            }
        }
        ApplyPositions();
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
                    if (positionsList.Count < resolution * resolution)
                    {
                        positionsList.Add(hit.point);
                        particleAmount++;
                        SetRandomColour();
                        laserLineRenderer.SetPosition(0, muzzlePoint.position);
                        laserLineRenderer.SetPosition(1, hit.point);
                    }
                    else
                    {
                        createNewVFX = true;
                        CreateVFX();
                        break;
                    }
                }
                ApplyPositions();
            }
            yield return new WaitForSeconds(0.05f);
        }
        laserLineRenderer.enabled = false; // Disable line renderer
        isScanning = false; // No longer scanning
        startRecharge = true; // Start rechargin
        StopCoroutine(LaserScan());
    }

    /// <summary>
    /// Overrides particle parameters and emits it.
    /// </summary>
    /// <param name="ps">Particle System to emit from.</param>
    /// <param name="contactPoint">Point where to emit the particle.</param>
    /// <param name="rotation">Rotation of the particle.</param>
    void DoEmit(ParticleSystem ps, Vector3 contactPoint, Vector3 rotation, float distance, bool hitEnemy)
    {
        // Any parameters we assign in emitParams will override the current system's when we call Emit.
        // Here we will override the position and velocity. All other parameters will use the behavior defined in the Inspector.
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.position = contactPoint;
        emitParams.velocity = Vector3.zero;
        emitParams.rotation3D = rotation;

        /*
        if (hitEnemy)
            emitParams.startColor = enemyHitColour;
        */
        emitParams.startColor = gradient.Evaluate(distance / farThreshhold);

        ps.Emit(emitParams, 1);
    }

    /// <summary>
    /// This function creates a new VFX Graph prefab.
    /// It creates a new prefab then sets the uINt resolution in VFX Graph object.
    /// Creates a texture2d with the same resolution and a new positions array made out of color.
    /// </summary>
    void CreateVFX()
    {
        if (!createNewVFX)
            return;

        vfxList.Add(currentVFX); // Add old prefab to the list

        currentVFX = Instantiate(vfxPrefab, transform.position, Quaternion.identity, vfxContainer.transform); // Create new vfx
        currentVFX.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution); // Assign the resolution

        texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false); // Create new texture2D
        positions = new Color[resolution * resolution]; // Create a new array of colours that will hold position Data 

        positionsList.Clear(); // Clear list of old positions.
        particleAmount = 0; // Reset particle amount

        createNewVFX = false;
    }

    /// <summary>
    /// This function applies position of particles.
    /// It takes a list of points and encodes them as Colors on a texture2D.
    /// </summary>
    void ApplyPositions()
    {
        Vector3[] pos = positionsList.ToArray(); // Take avalible point positions and transform to array.

        Vector3 vfxPos = currentVFX.transform.position; // POsition of our currently used VFX Graph object

        Vector3 transformPos = transform.position;

        int loopLength = texture.width * texture.height; // Loop through entire texture pixel by pixel.
        int posListLength = pos.Length; // Length of positions array.

        //Loop through texture
        for(int i = 0; i < loopLength; i++)
        {
            Color data;
            if(i < posListLength - 1) // If we can encode data
            {
                // Assign a pixel color with RGB values representing coordinates. 
                // Subtract position of VFX Graph to ensure the right position in world.
                data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1);
            }
            else
            {
                // If not data is avalible to encode create an invisible point to not make GPU angy.
                data = new Color(0,0,0,0);
            }
            positions[i] = data; // Save the encoded color.
        }

        texture.SetPixels(positions); // Set pixels to texture
        texture.Apply(); // Apply

        currentVFX.SetTexture(TEXTURE_NAME, texture); // Set texture in VFX Graph
        currentVFX.Reinit(); // Re initialize to display points.
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
