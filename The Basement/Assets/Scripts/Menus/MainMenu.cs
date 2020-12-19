using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
//    public static bool firstLoad = true;
//    public GameObject startGameButton;
    //public GameObject resumeButton;
    public void PlayGame()
    {
        if (!AudioController.firstLoad)
        {
            PlayerController.ResetStats();
            DungeonGenerator.reset = true;
        }
        AudioController.firstLoad = true;
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
