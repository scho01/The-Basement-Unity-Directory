using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;


public class RoomInfo
{
    public string name;
    public int X;
    public int Y;
}

public class RoomController : MonoBehaviour
{
    public static RoomController instance;
    string floorName = "Floor";
    public int currentFloorNum = 1;
    RoomInfo currentLoadRoomData;
    public Room currRoom;
    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();
    public List<Room> loadedRooms = new List<Room>();
    bool isLoadingRoom = false;
    bool spawnedBossRoom = false;
    bool updatedRooms = false;
    public Image blackScreen;
    public Text loadingText;
    public GameObject bossUI;
    public Text bossText;
    public Slider bossHealth;
    public bool bossDead = false;

    private void Awake()
    {
        instance = this;
        currentFloorNum = 1;
    }

    private void Start()
    {
        //LoadRoom("Start", 0, 0);
        //LoadRoom("Empty", -1, 0);
        //LoadRoom("Empty", 1, 0);
        //LoadRoom("Empty", 0, -1);
        //LoadRoom("Empty", 0, 1);
        if (currentFloorNum == 3)
            isLoadingRoom = true;
    }

    private void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if (isLoadingRoom)
        {
            return;
        }
        if (loadRoomQueue.Count == 0)
        {
            if (!spawnedBossRoom)
            {
                StartCoroutine(SpawnBossRoom());
            }
            else if (spawnedBossRoom && !updatedRooms)
            {
                foreach (Room room in loadedRooms)
                {
                    room.RemoveUnconnectedDoors();
                }
                UpdateRooms();
                updatedRooms = true;
            }
            return;
        }
        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;
        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }

    public IEnumerator FadeToBlack(bool ftb)
    {
        Color cColor = blackScreen.color;
        float fadeAmount;
        if (ftb)
        {
            loadingText.enabled = true;
            while (blackScreen.color.a < 1)
            {
                fadeAmount = cColor.a + (2 * Time.deltaTime);
                cColor = new Color(cColor.r, cColor.g, cColor.b, fadeAmount);
                blackScreen.color = cColor;
                yield return null;
            }
        }
        else
        {
            while (blackScreen.color.a > 0)
            {
                fadeAmount = cColor.a - (2 * Time.deltaTime);
                cColor = new Color(cColor.r, cColor.g, cColor.b, fadeAmount);
                blackScreen.color = cColor;
                yield return null;
            }
            loadingText.enabled = false;
        }
    }

    IEnumerator SpawnBossRoom()
    {
        spawnedBossRoom = true;
        yield return new WaitForSeconds(0.5f);
        if (loadRoomQueue.Count == 0)
        {
            Room bossRoom = loadedRooms[loadedRooms.Count - 1];
            Room tempRoom = new Room(bossRoom.X, bossRoom.Y);
            Destroy(bossRoom.gameObject);
            var roomToRemove = loadedRooms.Single(r => r.X == tempRoom.X && r.Y == tempRoom.Y);
            loadedRooms.Remove(roomToRemove);
            LoadRoom("End", tempRoom.X, tempRoom.Y);
            bossUI.SetActive(false);
            bossDead = false;
            PlayerMovement.vScreen = false;
            PlayerController.invulnerable = false;
            StartCoroutine(FadeToBlack(false));
        }
    }

    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y))
        {
            return;
        }
        RoomInfo newRoomData = new RoomInfo();
        newRoomData.name = name;
        newRoomData.X = x;
        newRoomData.Y = y;
        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        string roomName = floorName + currentFloorNum + info.name;
        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        yield return null;
        while (!loadRoom.isDone)
        {
            yield return null;
        }
    }

    public void RegisterRoom(Room room)
    {
        if (!DoesRoomExist(currentLoadRoomData.X, currentLoadRoomData.Y))
        {
            room.transform.position = new Vector3(
                currentLoadRoomData.X * room.Width,
                currentLoadRoomData.Y * room.Height, 0);
            room.X = currentLoadRoomData.X;
            room.Y = currentLoadRoomData.Y;
            room.name = floorName + currentFloorNum + "-" + currentLoadRoomData.name + " " + room.X + ", " + room.Y;
            room.transform.parent = transform;
            isLoadingRoom = false;
            if (loadedRooms.Count == 0)
            {
                CameraController.instance.currRoom = room;
            }
            loadedRooms.Add(room);
        }
        else
        {
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y) != null;
    }

    public Room FindRoom(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y);
    }

    public string GetRandomRoomName()
    {
        /*string[] possibleRooms = new string[]
        {
            "Empty",
            "Basic"
        };
        return possibleRooms[Random.Range(0, possibleRooms.Length)];*/
        int[] possibleRooms = new int[]     //Number of basic rooms per floor
        {
            6,
            10
        };
        if (Random.Range(0, 5) == 0)
            return "Empty";
        else
        {
            return "Basic " + Random.Range(0, possibleRooms[currentFloorNum - 1]);
        }
    }

    public void OnPlayerEnterRoom(Room room)
    {
        if (room != currRoom)
        {
            CameraController.instance.currRoom = room;
            currRoom = room;
            UpdateRooms();
            if (room.bossRoom && !bossDead)
                bossUI.SetActive(true);
            else
                bossUI.SetActive(false);
        }
    }

    private void UpdateRooms()
    {
        foreach(Room room in loadedRooms)
        {
            if (currRoom != room)
                room.enemiesObject.SetActive(false);
            else
            {
                room.enemiesObject.SetActive(true);
                EnemyController[] enemyControllers = room.enemiesObject.GetComponentsInChildren<EnemyController>();
                foreach (EnemyController ec in enemyControllers)
                {
                    ec.ForceIdle();
                }
            }
        }
    }

    public void UpdateBossHealth(string name, float current, float max)
    {
        bossText.text = name;
        bossHealth.maxValue = max;
        bossHealth.value = current;
    }
}
