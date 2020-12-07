using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int Width;
    public int Height;
    public int X;
    public int Y;
    private bool updatedDoors = false;

    public Room(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Door dDoor;
    public Door lDoor;
    public Door rDoor;
    public Door uDoor;
    public List<Door> doors = new List<Door>();
    public Wall dWall;
    public Wall lWall;
    public Wall rWall;
    public Wall uWall;
    //public List<Wall> walls = new List<Wall>();

    private void Start()
    {
        if (RoomController.instance == null)
        {
            Debug.Log("wrong scene");
            return;
        }
        Door[] ds = GetComponentsInChildren<Door>();
        foreach (Door d in ds)
        {
            doors.Add(d);
            switch (d.doorType)
            {
                case Door.DoorType.down:
                    dDoor = d;
                    break;
                case Door.DoorType.left:
                    lDoor = d;
                    break;
                case Door.DoorType.right:
                    rDoor = d;
                    break;
                case Door.DoorType.up:
                    uDoor = d;
                    break;
            }
        }
        Wall[] ws = GetComponentsInChildren<Wall>();
        foreach (Wall w in ws)
        {
            //walls.Add(w);
            switch (w.wallType)
            {
                case Wall.WallType.down:
                    dWall = w;
                    dWall.gameObject.SetActive(false);
                    break;
                case Wall.WallType.left:
                    lWall = w;
                    lWall.gameObject.SetActive(false);
                    break;
                case Wall.WallType.right:
                    rWall = w;
                    rWall.gameObject.SetActive(false);
                    break;
                case Wall.WallType.up:
                    uWall = w;
                    uWall.gameObject.SetActive(false);
                    break;
            }
        }
        RoomController.instance.RegisterRoom(this);
    }

    private void Update()
    {
        if (name.Contains("End") && !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
    }

    public void RemoveUnconnectedDoors()
    {
        foreach (Door door in doors)
        {
            switch (door.doorType)
            {
                case Door.DoorType.down:
                    if (GetDown() == null)
                    {
                        door.gameObject.SetActive(false);
                        dWall.gameObject.SetActive(true);
                    }
                    break;
                case Door.DoorType.left:
                    if (GetLeft() == null)
                    {
                        door.gameObject.SetActive(false);
                        lWall.gameObject.SetActive(true);
                    }
                    break;
                case Door.DoorType.right:
                    if (GetRight() == null)
                    {
                        door.gameObject.SetActive(false);
                        rWall.gameObject.SetActive(true);
                    }
                    break;
                case Door.DoorType.up:
                    if (GetUp() == null)
                    {
                        door.gameObject.SetActive(false);
                        uWall.gameObject.SetActive(true);
                    }
                    break;
            }
        }
    }

    public Room GetDown()
    {
        if (RoomController.instance.DoesRoomExist(X, Y - 1))
            return RoomController.instance.FindRoom(X, Y - 1);
        return null;
    }

    public Room GetLeft()
    {
        if (RoomController.instance.DoesRoomExist(X - 1, Y))
            return RoomController.instance.FindRoom(X - 1, Y);
        return null;
    }

    public Room GetRight()
    {
        if (RoomController.instance.DoesRoomExist(X + 1, Y))
            return RoomController.instance.FindRoom(X + 1, Y);
        return null;
    }

    public Room GetUp()
    {
        if (RoomController.instance.DoesRoomExist(X, Y + 1))
            return RoomController.instance.FindRoom(X, Y + 1);
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0));
    }

    public Vector3 GetRoomCenter()
    {
        return new Vector3(X * Width, Y * Height, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision");
        if (collision.CompareTag("Player"))
        {
            RoomController.instance.OnPlayerEnterRoom(this);
        }
    }
}
