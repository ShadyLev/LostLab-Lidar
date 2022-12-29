using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenu : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private bool isOpened = false;
    public bool IsPauseMenuOpened { get { return isOpened; } }

    [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] GameObject mapMenuCanvas;
    [SerializeField] MapManager mapManager;
    [SerializeField] PlayerInput playerInput;

    [Header("Volumes")]
    [SerializeField] GameObject normalGameVolume;
    [SerializeField] GameObject menuUIVolume;

    [Header("Cameras")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera menuCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !isOpened)
            TogglePauseMenuCanvas();
    }

    public void TogglePauseMenuCanvas()
    {
        
        isOpened = !isOpened;

        //Debug.Log(PlayerStatus.usingUI);
        pauseMenuCanvas.SetActive(isOpened);
        Time.timeScale = isOpened ? 0 : 1;

        if (isOpened)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            normalGameVolume.SetActive(false);
            menuUIVolume.SetActive(true);

            if (mapManager.IsMapOpened)
                mapMenuCanvas.SetActive(false);

            playerCamera.enabled = false;
            menuCamera.enabled = true;
            playerInput.enabled = false;

        }else if (!isOpened)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            normalGameVolume.SetActive(true);
            menuUIVolume.SetActive(false);

            if (mapManager.IsMapOpened)
            {
                mapMenuCanvas.SetActive(true);
                playerInput.enabled = false;
            }
            else
            {
                playerInput.enabled = true;
            }

            playerCamera.enabled = true;
            menuCamera.enabled = false;
        }
    }
}
