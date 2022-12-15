using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseMenu : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;
    private bool isOpened = false;
    [SerializeField] GameObject pauseMenuCanvas;

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
    }
}
