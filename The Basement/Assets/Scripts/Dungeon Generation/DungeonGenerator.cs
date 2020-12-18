using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonGenerationData;
    public DungeonGenerationData dungeonGenerationData2;
    private List<Vector2Int> dungeonRooms;
    public static bool reset = true;

    private void Start()
    {
        StartCoroutine(RoomController.instance.FadeToBlack(true));
        if (RoomController.instance.currentFloorNum == 1)
            dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);
        else
            dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData2);
        SpawnRooms(dungeonRooms);
    }

    private void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        if (reset)
        {
            RoomController.instance.LoadRoom("Start", 0, 0);
            RoomController.instance.currentFloorNum = 1;
            reset = false;
        }
        else
        {
            RoomController.instance.currentFloorNum = 2;
            RoomController.instance.LoadRoom("Empty", 0, 0);
        }
        foreach (Vector2Int roomLocation in rooms)
            RoomController.instance.LoadRoom(RoomController.instance.GetRandomRoomName(), roomLocation.x, roomLocation.y);
    }
}
