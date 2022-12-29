using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using System;

/// <summary>
/// This class controls the behaviour and settings of VFX Graph.
/// </summary>
/// <remarks>Written by Benedykt Cieslinski</remarks>
public class VFXGraphManager : MonoBehaviour
{
    #region Variables

    [Header("Particle Systems")]
    [Tooltip("Player object transform to track for accurate gradient color data.")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("Prefab with a single instance of the VFX Graph component.")]
    [SerializeField] VisualEffect vfxPrefab;
    [Tooltip("Parent object for VFX Graph systems.")]
    [SerializeField] GameObject vfxContainer;
    [Tooltip("Particle texture to use.")]
    [SerializeField] Texture2D particleTexture;

    //----VFX GRAPH VARIABLE REFERENCES----
    private const string MAX_PARTICLE_COUNT_PARAMETER_NAME = "MaxParticleCount"; // Reference to VFX Graph variable
    private const string VECTOR3_PLAYER_NAME = "PlayerPos"; // Reference to VFX Graph variable
    private const string PARTICLE_TEXTURE_NAME = "ParticleTexture"; // Reference to VFX Graph variable
    private const string GRADIENT_NAME = "DefaultGradient"; // Reference to VFX Graph variable
    private const string MAX_DISTANCE_COLOR_NAME = "MaxDistanceColor"; // Reference to VFX Graph variable

    //---Hidden Variables----
    private Gradient defaultParticleGradient; // Gradient to use for particles
    public Gradient DefaultParticleGradient
    {
        get { return this.defaultParticleGradient; }
        set { this.defaultParticleGradient = value; }
    }
    private float gradientMaxDistance; // Max distance for particle gradient
    public float GradientMaxDistance
    {
        get { return this.gradientMaxDistance; }
        set { this.gradientMaxDistance = value; }
    }

    private List<VisualEffect> m_vfxList = new List<VisualEffect>(); // List of VFX Graphs
    private VisualEffect m_currentVFX; // Current used VFX Graph

    private int m_VFXParticleAmount; // Current amount of particles
    private int maxParticleCount = 10000; // Particle count per system

    /// VFX GRAPH BUFFER VARIABLES
    private GraphicsBuffer gfxBuffer; // Graphics Buffer
    private int m_BufferPropertyID = Shader.PropertyToID("CustomBuffer"); // VFX Graph Buffer ID

    // Custom Buffer Data
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    struct CustomVFXData
    {
        public Vector3 position;
        public Vector4 color;
        public int useDefaultGradient;
        public float size;
    }

    // List of custom Data points
    private List<CustomVFXData> m_CustomVFXData = new List<CustomVFXData>();

    #endregion

    #region Awake, Start & Update

    // Start is called before the first frame update
    void Start()
    {
        CreateVFX();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerPosVFXGraph();
    }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Destroys all VFX Objects from the list and clears the list.
    /// </summary>
    public void DestroyVFXList()
    {
        List<VisualEffect> tmpVs = m_vfxList;

        foreach (VisualEffect vs in tmpVs)
        {
            Destroy(vs.gameObject);
        }

        m_vfxList.Clear();

        m_VFXParticleAmount = 0;

        Release();

        CreateVFX();
    }

    /// <summary>
    /// Adds particle data to the list of CustomVFXData.
    /// </summary>
    /// <param name="position">Position of the particle</param>
    /// <param name="color">Color of the particle.</param>
    /// <param name="lifetime">Lifetime of the particle.</param>
    public void AddDataToBuffer(Vector3 position, Vector4 color, int useDefaultGradient, float size)
    {
        // Check if there is space to add new data to buffer.
        if (CheckIfBufferFull())
            return;

        // Create new CustomVFX Data
        CustomVFXData newData = new CustomVFXData();

        // Apply data
        newData.position = position;
        newData.color = color;
        newData.useDefaultGradient = useDefaultGradient;
        newData.size = size;

        // Add to list
        m_CustomVFXData.Add(newData);

        // Increase particle amount
        m_VFXParticleAmount++;
    }


    /// <summary>
    /// Sets custom buffer data to the Graphics Buffer and Re Initilizes current VFX Graph
    /// </summary>
    public void SetCustomBufferData()
    {
        gfxBuffer.SetData(m_CustomVFXData);

        m_currentVFX.Reinit();
    }

    /// <summary>
    /// Gives current particle count.
    /// </summary>
    /// <returns>Int of particles.</returns>
    public int GetCurrentParticleCount()
    {
        return m_VFXParticleAmount;
    }

    /// <summary>
    /// Sets the default particle gradoent to use.
    /// </summary>
    /// <param name="gradient">Default particle gradient.</param>
    /// <param name="maxDistance">Max gradient distance.</param>
    public void SetDefaultGradientData(Gradient gradient, float maxDistance)
    {
        defaultParticleGradient = gradient;
        gradientMaxDistance = maxDistance;
    }

    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Creates a new graphics buffer of size.
    /// </summary>
    /// <param name="newSize">Graphics buffer count.</param>
    void CreateNewBuffer(int newSize)
    {
        if (gfxBuffer != null)
            gfxBuffer.Release();

        gfxBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, newSize, Marshal.SizeOf(typeof(CustomVFXData)));
        gfxBuffer.SetData(new CustomVFXData[newSize]);
    }

    /// <summary>
    /// Checks if Graphics Buffer is full.
    /// If so -> create new VFX Graph object and clear buffer data.
    /// </summary>
    bool CheckIfBufferFull()
    {
        if (m_CustomVFXData.Count + 1 > gfxBuffer.count)
        {
            CreateVFX();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// This function creates a new VFX Graph prefab.
    /// It creates a new prefab then sets the uINt resolution in VFX Graph object.
    /// Creates a texture2d with the same resolution and a new positions array made out of color.
    /// </summary>
    private void CreateVFX()
    {
        CreateNewBuffer(maxParticleCount); // Create new Graphics buffer

        m_CustomVFXData.Clear(); // Clear buffer data

        m_currentVFX = Instantiate(vfxPrefab, transform.position, Quaternion.identity, vfxContainer.transform); // Create new vfx

        m_currentVFX.SetUInt(MAX_PARTICLE_COUNT_PARAMETER_NAME, (uint)maxParticleCount); // Assign the resolution

        m_currentVFX.SetTexture(PARTICLE_TEXTURE_NAME, particleTexture); // Assign particle texture

        m_currentVFX.SetGradient(GRADIENT_NAME, defaultParticleGradient); // Set default gradient

        m_currentVFX.SetFloat(MAX_DISTANCE_COLOR_NAME, gradientMaxDistance); // Set max distance colour var

        m_currentVFX.SetGraphicsBuffer(m_BufferPropertyID, gfxBuffer); // Set graphics buffer

        m_vfxList.Add(m_currentVFX); // Add old prefab to the list
    }

    /// <summary>
    /// Updates the player position in every VFX graph.
    /// </summary>
    private void UpdatePlayerPosVFXGraph()
    {
        foreach (VisualEffect vs in m_vfxList)
        {
            vs.SetVector3(VECTOR3_PLAYER_NAME, playerTransform.position - vs.transform.position);
        }
    }

    #endregion

    #region Dispose
    public void OnDisable()
    {
        Release();
    }

    public void OnDestroy()
    {
        Release();
    }

    void Release()
    {
        if(gfxBuffer!= null)
            gfxBuffer.Release();

        gfxBuffer = null;
    }
    #endregion
}
