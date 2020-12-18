using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySceneController : MonoBehaviour
{
    private void Update()
    {
        PlayerController.invulnerable = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerHitBox"))
        {
            ConfirmationDialog.Show($"Are you sure you want to return to the Main Menu?\n\n<size=80>Y / N</size>", () =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}
