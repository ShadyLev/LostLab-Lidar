using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Runtime.InteropServices;

public class VFXGraphManager : MonoBehaviour
{
    [Header("Particle Systems")]
    private const string RESOLUTION_PARAMETER_NAME = "Resolution"; // Reference to VFX Graph variable
    private const string VECTOR3_PLAYER_NAME = "PlayerPos"; // Reference to VFX Graph variable

    private List<Vector3> positionsList = new List<Vector3>(); // List holding all positions of hit points 
    public List<VisualEffect> vfxList = new List<VisualEffect>(); // List of VFX Graphs
    [SerializeField] private VisualEffect currentVFX; // Current used VFX Graph
    private Texture2D texture; // Texture holding DATA info
    private Color[] positions; // Positions encoded in a Color array
    [SerializeField] private int particleAmount; // Current amount of particles
    private bool createNewVFX = false; // Create new vfx bool

    [SerializeField] VisualEffect vfxPrefab; // VFX Graph prefab
    [SerializeField] GameObject vfxContainer; // Gameobject holding all VFX Graph prefabs in scene

    private int resolution = 16384; // Resolution of the texture2d holding data points

    [SerializeField] private Transform playerTransform;

    /// VFX GRAPH BUFFER VARIABLES

    public GraphicsBuffer gfxBuffer;
    private int m_BufferPropertyID = Shader.PropertyToID("CustomBuffer");

    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    struct CustomVFXData
    {
        public Vector3 position;
        public Vector4 color;
        public float lifetime;
    }

    private List<CustomVFXData> m_CustomVFXData = new List<CustomVFXData>();

    void Reallocate(int newSize)
    {
        if (gfxBuffer != null)
            gfxBuffer.Release();

        gfxBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, newSize, Marshal.SizeOf(typeof(CustomVFXData)));
        gfxBuffer.SetData(new CustomVFXData[newSize]);
    }

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

        //SetCustomBufferData();
    }

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

    public void AddDataToBuffer(Vector3 position, Vector4 color)
    {
        CheckIfBufferFull();

        CustomVFXData newData = new CustomVFXData();

        newData.position = position;
        newData.color = color;

        m_CustomVFXData.Add(newData);
        particleAmount++;
    }

    public void SetCustomBufferData()
    {
        gfxBuffer.SetData(m_CustomVFXData);

        currentVFX.Reinit();
    }

    /// <summary>
    /// This function creates a new VFX Graph prefab.
    /// It creates a new prefab then sets the uINt resolution in VFX Graph object.
    /// Creates a texture2d with the same resolution and a new positions array made out of color.
    /// </summary>
    public void CreateVFX()
    {
        currentVFX = Instantiate(vfxPrefab, transform.position, Quaternion.identity, vfxContainer.transform); // Create new vfx

        currentVFX.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution); // Assign the resolution

        currentVFX.SetGraphicsBuffer(m_BufferPropertyID, gfxBuffer);

        vfxList.Add(currentVFX); // Add old prefab to the list

        particleAmount = 0; // Reset particle amount

        m_CustomVFXData.Clear(); // Clear buffer data
    }

    /// <summary>
    /// Updates the player position in every VFX graph.
    /// </summary>
    void UpdatePlayerPosVFXGraph()
    {
        foreach (VisualEffect vs in vfxList)
        {
            vs.SetVector3(VECTOR3_PLAYER_NAME, playerTransform.position - vs.transform.position);
        }
    }

    /// <summary>
    /// Destroys a specific VFX Graph object from the list of VFX Graphs.
    /// </summary>
    /// <param name="vs">Visual Effect to destroy</param>
    public void DestroyOneFromVFXList(VisualEffect vs)
    {
        vfxList.Remove(vs);
        CreateVFX();
    }

    /// <summary>
    /// Destroys all VFX Objects from the list and clears the list.
    /// </summary>
    public void DestroyVFXList()
    {
        List<VisualEffect> tmpVs = vfxList;
        foreach(VisualEffect vs in tmpVs)
        {
            Destroy(vs.gameObject);
        }
        vfxList.Clear();
        CreateVFX();
    }

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
