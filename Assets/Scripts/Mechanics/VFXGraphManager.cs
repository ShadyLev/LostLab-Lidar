using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using System;

public class VFXGraphManager : MonoBehaviour
{
    #region Variables

    [Header("Particle Systems")]
    private const string RESOLUTION_PARAMETER_NAME = "Resolution"; // Reference to VFX Graph variable
    private const string VECTOR3_PLAYER_NAME = "PlayerPos"; // Reference to VFX Graph variable

    [SerializeField] List<VisualEffect> m_vfxList = new List<VisualEffect>(); // List of VFX Graphs
    [SerializeField] private VisualEffect m_currentVFX; // Current used VFX Graph
    [SerializeField] private int m_particleAmount; // Current amount of particles

    [SerializeField] VisualEffect vfxPrefab; // VFX Graph prefab
    [SerializeField] GameObject vfxContainer; // Gameobject holding all VFX Graph prefabs in scene

    private int resolution = 16384; // Resolution of the texture2d holding data points

    [SerializeField] private Transform playerTransform;

    /// VFX GRAPH BUFFER VARIABLES

    private GraphicsBuffer gfxBuffer;
    private int m_BufferPropertyID = Shader.PropertyToID("CustomBuffer");

    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    struct CustomVFXData
    {
        public Vector3 position;
        public Vector4 color;
        public float lifetime;
    }

    private List<CustomVFXData> m_CustomVFXData = new List<CustomVFXData>();

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Reallocate(resolution);

        CreateVFX();
        //ApplyPositions();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerPosVFXGraph();
    }

    #region PUBLIC METHODS

    /// <summary>
    /// Destroys a specific VFX Graph object from the list of VFX Graphs.
    /// </summary>
    /// <param name="vs">Visual Effect to destroy</param>
    public void DestroyOneFromVFXList(VisualEffect vs)
    {
        m_vfxList.Remove(vs);
        CreateVFX();
    }

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
        CreateVFX();
    }

    /// <summary>
    /// Adds particle data to the list of CustomVFXData.
    /// </summary>
    /// <param name="position">Position of the particle</param>
    /// <param name="color">Color of the particle.</param>
    /// <param name="lifetime">Lifetime of the particle.</param>
    public void AddDataToBuffer(Vector3 position, Vector4 color, float lifetime)
    {
        // Check if there is space to add new data to buffer.
        CheckIfBufferFull(); 

        // Create new CustomVFX Data
        CustomVFXData newData = new CustomVFXData();

        // Apply data
        newData.position = position;
        newData.color = color;
        newData.lifetime = lifetime;

        // Add to list
        m_CustomVFXData.Add(newData);

        // Increase particle amount
        m_particleAmount++;
    }


    /// <summary>
    /// Sets custom buffer data to the Graphics Buffer and Re Initilizes current VFX Graph
    /// </summary>
    public void SetCustomBufferData()
    {
        gfxBuffer.SetData(m_CustomVFXData);

        m_currentVFX.Reinit();
    }

    #endregion

    #region PRIVATE METHODS

    void Reallocate(int newSize)
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
    void CheckIfBufferFull()
    {
        // Reallocate more data if needed
        if (m_CustomVFXData.Count > gfxBuffer.count)
        {
            int newCount = gfxBuffer.count;
            while (newCount < m_CustomVFXData.Count)
                newCount *= 2;

            m_CustomVFXData.Clear();
            //Reallocate(resolution);
            CreateVFX();
        }
    }

    /// <summary>
    /// This function creates a new VFX Graph prefab.
    /// It creates a new prefab then sets the uINt resolution in VFX Graph object.
    /// Creates a texture2d with the same resolution and a new positions array made out of color.
    /// </summary>
    private void CreateVFX()
    {
        m_currentVFX = Instantiate(vfxPrefab, transform.position, Quaternion.identity, vfxContainer.transform); // Create new vfx

        m_currentVFX.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution); // Assign the resolution

        m_currentVFX.SetGraphicsBuffer(m_BufferPropertyID, gfxBuffer);

        m_vfxList.Add(m_currentVFX); // Add old prefab to the list

        m_particleAmount = 0; // Reset particle amount

        m_CustomVFXData.Clear(); // Clear buffer data
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
        if (gfxBuffer != null)
        {
            gfxBuffer.Release();
            gfxBuffer = null;
        }
    }
    #endregion
}
