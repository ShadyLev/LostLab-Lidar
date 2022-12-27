using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenu : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private bool isOpened = false;
    [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] PlayerInput playerInput;

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

        playerInput.enabled = !isOpened;

        if (isOpened)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }else if (!isOpened)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
