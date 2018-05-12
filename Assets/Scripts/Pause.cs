using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    bool paused = false;

    void Update()
    {
        //Cursor.visible = false;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = togglePause();
        }
    }

    void OnGUI()
    {
        if (paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GUILayout.Label("Game is paused!");
            if (GUILayout.Button("Click me to unpause"))
            {
                paused = togglePause();
            }
            if(GUILayout.Button("Return to main menu"))
            {
                SceneManager.LoadScene(0);
            }
            if(GUILayout.Button("Quit the game"))
            {
                Application.Quit();
            }   
            
        }
    }

    bool togglePause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            return (false);
        }
        else
        {
            Time.timeScale = 0f;
            return (true);
        }
    }
}
