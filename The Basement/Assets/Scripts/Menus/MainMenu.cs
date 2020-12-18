using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private static bool firstLoad = true;
    public GameObject startGameButton;
    public GameObject resumeButton;
    public void PlayGame()
    {
        if (!firstLoad)
        {
            PlayerController.ResetStats();
            DungeonGenerator.reset = true;
        }
        firstLoad = false;
        SceneManager.LoadScene(1);
    }

    public void ResumeGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
