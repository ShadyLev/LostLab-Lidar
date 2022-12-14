using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXGraphManager : MonoBehaviour
{
    [Header("Particle Systems")]
    private const string TEXTURE_NAME = "PositionsTexture"; // Reference to VFX Graph variable
    private const string RESOLUTION_PARAMETER_NAME = "Resolution"; // Reference to VFX Graph variable
    private const string VECTOR3_NAME = "PlayerPos"; // Reference to VFX Graph variable

    private List<Vector3> positionsList = new List<Vector3>(); // List holding all positions of hit points 
    public List<VisualEffect> vfxList = new List<VisualEffect>(); // List of VFX Graphs
    private VisualEffect currentVFX; // Current used VFX Graph
    private Texture2D texture; // Texture holding DATA info
    private Color[] positions; // Positions encoded in a Color array
    [SerializeField] private int particleAmount; // Current amount of particles
    private bool createNewVFX = false; // Create new vfx bool

    [SerializeField] VisualEffect vfxPrefab; // VFX Graph prefab
    [SerializeField] GameObject vfxContainer; // Gameobject holding all VFX Graph prefabs in scene

    private int resolution = 100; // Resolution of the texture2d holding data points

    // Start is called before the first frame update
    void Start()
    {
        createNewVFX = true;

        CreateVFX();
        ApplyPositions();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerPosVFXGraph();
    }

    public void AddPositions(Vector3 position)
    {
        positionsList.Add(position);
        particleAmount++;
    }

    public bool CheckIfCanAddData()
    {
        if (positionsList.Count < resolution * resolution)
        {
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
    public void CreateVFX()
    {
        currentVFX = Instantiate(vfxPrefab, transform.position, Quaternion.identity, vfxContainer.transform); // Create new vfx

        currentVFX.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution); // Assign the resolution

        vfxList.Add(currentVFX); // Add old prefab to the list

        texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false); // Create new texture2D
        positions = new Color[resolution * resolution]; // Create a new array of colours that will hold position Data 

        positionsList.Clear(); // Clear list of old positions.
        particleAmount = 0; // Reset particle amount
    }

    /// <summary>
    /// This function applies position of particles.
    /// It takes a list of points and encodes them as Colors on a texture2D.
    /// </summary>
    public void ApplyPositions()
    {
        Vector3[] pos = positionsList.ToArray(); // Take avalible point positions and transform to array.

        Vector3 vfxPos = currentVFX.transform.position; // POsition of our currently used VFX Graph object

        Vector3 transformPos = transform.position;

        int loopLength = texture.width * texture.height; // Loop through entire texture pixel by pixel.
        int posListLength = pos.Length; // Length of positions array.

        //Loop through texture
        for (int i = 0; i < loopLength; i++)
        {
            Color data;
            if (i < posListLength - 1) // If we can encode data
            {
                // Assign a pixel color with RGB values representing coordinates. 
                // Subtract position of VFX Graph to ensure the right position in world.
                data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1);
            }
            else
            {
                // If not data is avalible to encode create an invisible point to not make GPU angy.
                data = new Color(0, 0, 0, 0);
            }
            positions[i] = data; // Save the encoded color.
        }

        texture.SetPixels(positions); // Set pixels to texture
        texture.Apply(); // Apply

        currentVFX.SetTexture(TEXTURE_NAME, texture); // Set texture in VFX Graph
        currentVFX.Reinit(); // Re initialize to display points.
    }

    void UpdatePlayerPosVFXGraph()
    {
        foreach (VisualEffect vs in vfxList)
        {
            vs.SetVector3(VECTOR3_NAME, transform.position);
        }
    }
}
