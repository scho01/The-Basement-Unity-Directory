using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LadderController : MonoBehaviour
{
    public string ladderName = "Ladder";
    public string ladderDescription = "Move on to the next floor.";
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
        SceneManager.LoadScene(0);
    }
}
