using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    [Header("Somrthing")]
    [Tooltip("Array that holds the room walls")]
    public GameObject[] walls; 
    // 0 - Up
    // 1 - Down
    // 2 - Right
    // 3 - Left

    [Tooltip("Array that holds the room doors")]
    public GameObject[] doors;

    [Tooltip("Int showing the difficulty of the room")]
    public int roomDifficulty;
    // 0 - Easy
    // 1 - Medium
    // 2 - Hard
    // 3 - Boss

    /// <summary>
    /// Updates the room door status
    /// </summary>
    /// <param name="status">Status of the room doors (open or closed)</param>
    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]); // If true activate door model
            walls[i].SetActive(!status[i]); // If false activate the wall model
        }
    }
}
