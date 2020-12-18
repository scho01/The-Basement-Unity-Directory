using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LadderController : MonoBehaviour
{
    public string ladderName = "Ladder";
    public string ladderDescription = "Move on to the next floor.";
    public string reverseDescription = "Restart the game from the first floor (Your current stats will be retained)";
    public bool hover;
    private PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hover)
        {
            if (collision.CompareTag("Player"))
            {
                player.numItems++;
                player.items.Add(gameObject);
                hover = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hover)
        {
            if (collision.CompareTag("Player"))
            {
                player.numItems--;
                player.items.Remove(gameObject);
                hover = false;
            }
        }
    }

    public void Use()
    {
        if (RoomController.instance.currentFloorNum > 2)
        {
            ConfirmationDialog.Show($"Are you sure you want to return to the first floor?\nYour current stats will be retained.\n\n<size=80>Y / N</size>", () =>
            {
                RoomController.instance.currentFloorNum++;
                DungeonGenerator.reset = true;
                PlayerMovement.vScreen = false;
                PlayerController.invulnerable = false;
                SceneManager.LoadScene(1);
            });
        }
        else
        {
            ConfirmationDialog.Show($"Proceed to the next floor?\n\n<size=80>Y / N</size>", () =>
            {
                RoomController.instance.currentFloorNum++;
                if (RoomController.instance.currentFloorNum < 3)
                    SceneManager.LoadScene(1);
                else
                {
                    SceneManager.LoadScene(2);
                    PlayerMovement.vScreen = true;
                }
            });
        }
    }
}
