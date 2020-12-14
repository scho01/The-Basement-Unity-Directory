using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        down, left, right, up
    }
    private GameObject player;
    public DoorType doorType;
    private float widthOffset = 1.5f;
    public GameObject doorCollider;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            switch(doorType)
            {
                case DoorType.down:
                    player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - widthOffset);
                    break;
                case DoorType.left:
                    player.transform.position = new Vector2(player.transform.position.x - widthOffset, player.transform.position.y);
                    break;
                case DoorType.right:
                    player.transform.position = new Vector2(player.transform.position.x + widthOffset, player.transform.position.y);
                    break;
                case DoorType.up:
                    player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + widthOffset);
                    break;
            }
        }
    }
}
