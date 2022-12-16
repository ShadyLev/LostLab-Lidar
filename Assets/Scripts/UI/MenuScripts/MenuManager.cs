using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject settingsCanvas;
    public GameObject introCanvas;
    public string mainLevelName;
    public Color normalTextColor;
    public Color hoverTextColor;

    // Update is called once per frame
    void Update()
    {
        ClickBackButton();
    }

    void ClickBackButton()
    {
        // If player pressed the close button (Esc as default)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject button = GameObject.Find("BackButton"); // Find the current active button
            if (button != null) // If it's not null
            {
                Debug.Log("Hit back button", button);
                button.GetComponent<Button>().onClick.Invoke(); // Invoke the onClick event of the button
            }
        }
    }

    public void StartSinglePlayerGame()
    {
        SceneManager.LoadScene(mainLevelName);
    }

    public void StartIntro()
    {
        menuCanvas.SetActive(false);
        introCanvas.SetActive(true);
    }
    
    public void BackFromIntro()
    {
        introCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSettings()
    {
        menuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void BackFromSettings()
    {
        menuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void OnHoverEnterColourChange(TextMeshProUGUI text)
    {
        text.color = hoverTextColor;
        // Play on hover sound
        AudioManager.Instance.ButtonHover();
    }

    public void OnHoverExitColourChange(TextMeshProUGUI text)
    {
        text.color = normalTextColor;
    }

    public void OnDownClick(TextMeshProUGUI text)
    {
        text.color = normalTextColor;
        // Stuff
        AudioManager.Instance.ButtonClick();
    }
}
