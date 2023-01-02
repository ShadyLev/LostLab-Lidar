using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the player input for the scanner.
/// </summary>
/// <remarks>Written by Benedykt Cieslinski.</remarks>
public class PlayerInput : MonoBehaviour
{
    [Tooltip("Reference to the scanner script.")]
    [SerializeField] LIDARScanner scanner;

    [Tooltip("Key that when held will perform a normal scan.")]
    [SerializeField] KeyCode m_normalScanKey;
    [Tooltip("Key that when pressed will perform a big scan.")]
    [SerializeField] KeyCode m_bigScanKey;
    [Tooltip("How much the radius changes when scrolling")]
    [Range(0, 1)]
    [SerializeField] private float m_scrollValue;
    [Tooltip("Input axis that will control the scanner radius change.")]
    [SerializeField] string m_radiusChangeAxis;

    // Start is called before the first frame update
    void Start()
    {
        // Apply the scroll value to the scanner script.
        scanner.ScrollValue = m_scrollValue;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    /// <summary>
    /// Gets the player inputs
    /// </summary>
    public void GetInput()
    {
        // Adjust the 
        scanner.AdjustNormalScanRadius(m_radiusChangeAxis);

        // If big scan is active do not progress forward
        if (scanner.IsBigScanning)
            return;

        //Check if big scan key is pressed and perform scan
        if (Input.GetKeyDown(m_bigScanKey) && !scanner.IsNormalScanning)
        {
            scanner.StartBigScan();
        }

        // While pressed scan
        if (Input.GetKey(m_normalScanKey))
        {
            scanner.StartNormalScan();
        }
        else if (Input.GetKeyUp(m_normalScanKey)) // On key up end the scan
        {
            scanner.EndNormalScan();
        }
    }
}
