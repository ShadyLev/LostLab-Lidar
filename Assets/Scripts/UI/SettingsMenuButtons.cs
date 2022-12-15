using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuButtons : MonoBehaviour
{
    // Canvases refrences
    public GameObject howToPlay;
    public GameObject graphics;
    public GameObject controls;
    public GameObject audioObject;

    // Main buttons reference
    public GameObject mainButtons;

    /// <summary>
    /// Opens the how to play canvas
    /// </summary>
    public void OpenHowtoPlay()
    {
        howToPlay.SetActive(true);
        graphics.SetActive(false);
        controls.SetActive(false);
        audioObject.SetActive(false);
        mainButtons.SetActive(false);
    }

    /// <summary>
    /// Opens the graphics canvas
    /// </summary>
    public void OpenGraphics()
    {
        howToPlay.SetActive(false);
        graphics.SetActive(true);
        controls.SetActive(false);
        audioObject.SetActive(false);
        mainButtons.SetActive(false);
    }

    /// <summary>
    /// Opens the controls canvas
    /// </summary>
    public void OpenControls()
    {
        howToPlay.SetActive(false);
        graphics.SetActive(false);
        controls.SetActive(true);
        audioObject.SetActive(false);
        mainButtons.SetActive(false);
    }

    /// <summary>
    /// Opens the audio canvas
    /// </summary>
    public void OpenAudio()
    {
        howToPlay.SetActive(false);
        graphics.SetActive(false);
        controls.SetActive(false);
        audioObject.SetActive(true);
        mainButtons.SetActive(false);
    }

    /// <summary>
    /// Opens the main canvas
    /// </summary>
    public void BackFromCanvas(GameObject backFromCanvas)
    {
        mainButtons.SetActive(true);
        backFromCanvas.SetActive(false);
    }
}
