using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DungeonCell
{
    public bool NorthExit { get; set; }
    public bool SouthExit { get; set; }
    public bool EastExit { get; set; }
    public bool WestExit { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class GenerationManager : MonoBehaviour
{
    public List<DungeonCell> possibleCells = new List<DungeonCell>();

    private bool[,] dungeon;
    private GameObject mazeObject;
    public int width;
    public int height;
    public int generationAttempts;

    public int maxRoomWidth;
    public int maxRoomHeight;
    public int minRooms;
    public int maxRooms;


    public void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(GenerateGuessMaze(width, height));
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            StartCoroutine(GenerateStemmingMaze(width, height));
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            StartCoroutine(DwarfGen(width, height, maxRoomWidth, maxRoomHeight, minRooms, maxRooms));
        }
    }

    struct DwarfGenComplex
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public List<DwarfGenComplex> connectedRooms;
    }

    public IEnumerator DwarfGen(int width, int height, int roomWidth, int roomHeight, int minRooms, int maxRooms) {
        if (mazeObject != null) {
            Destroy(mazeObject);
        }
        mazeObject = new GameObject();
        this.width = width;
        this.height = height;
        dungeon = new bool[width, height];

        List<DwarfGenComplex> rooms = new List<DwarfGenComplex>();
        int numRooms = Random.Range(minRooms, maxRooms);

        for (int i = 0; i < numRooms; i++) {
            DwarfGenComplex room = new DwarfGenComplex();
            room.width = Random.Range(2, roomWidth);
            room.height = Random.Range(2, roomHeight);
            room.x = Random.Range(0, width - room.width);
            room.y = Random.Range(0, height - room.height);
            room.connectedRooms = new List<DwarfGenComplex>();
            rooms.Add(room);
        }

        foreach (DwarfGenComplex room in rooms) {
            Debug.LogFormat("Room: x: {0}, y: {1}, width: {2}, height: {3}", room.x, room.y, room.width, room.height);
            for (int i = room.x; i < room.x + room.width; i++) {
                for (int j = room.y; j < room.y + room.height; j++) {
                    dungeon[i, j] = true;
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.transform.parent = mazeObject.transform;
                    obj.transform.position = new Vector3(i, 0, j);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator GenerateStemmingMaze(int width, int height) {
        if (mazeObject != null) {
            Destroy(mazeObject);
        }
        mazeObject = new GameObject();
        this.width = width;
        this.height = height;
        dungeon = new bool[width, height];
        dungeon[width / 2, height / 2] = true;
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.parent = mazeObject.transform;
        obj.transform.position = new Vector3(width / 2, 0, height / 2);

        for (int i = 0; i < generationAttempts; i++) {
            Vector2 existingCell = randomExistingNotSurrounded();
            List<Vector2> possible = OpenNeighbors((int)existingCell.x, (int)existingCell.y);
            Vector2 newCell = possible[Random.Range(0, possible.Count)];
            Debug.LogFormat("Existing: {0}, New: {1}", existingCell, newCell);
            dungeon[(int)newCell.x, (int)newCell.y] = true;
            int mirrorX = width - 1 - (int)newCell.x;
            int mirrorY = height - 1 - (int)newCell.y;
            GameObject newRoom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newRoom.transform.position = new Vector3(newCell.x, 0, newCell.y);
            newRoom.transform.parent = mazeObject.transform;
            if (adjacentToRoom(mirrorX, mirrorY) && Random.Range(0.0f, 1.0f) > .05f) {
                dungeon[mirrorX, mirrorY] = true;
                newRoom = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newRoom.transform.position = new Vector3(mirrorX, 0, mirrorY);
                newRoom.transform.parent = mazeObject.transform;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator GenerateGuessMaze(int width, int height)
    {
        if (mazeObject != null)
        {
            Destroy(mazeObject);
        }
        mazeObject = new GameObject();
        this.width = width;
        this.height = height;
        dungeon = new bool[width, height];
        dungeon[width / 2, height / 2] = true;
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.parent = mazeObject.transform;
        obj.transform.position = new Vector3(width/2, 0, height/2);
        for (int i = 0; i < generationAttempts; i++)
        {
            int x = Random.Range(0, width-1);
            int y = Random.Range(0, height-1);
            if (!dungeon[x,y] && adjacentToRoom(x,y))
            {
                dungeon[x, y] = true;
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(x, 0, y);
                obj.transform.parent = mazeObject.transform;
                if (Random.Range(0.0f,1.0f) > 0.2f)
                {
                    int mirrorX = width - 1 - x;
                    int mirrorY = height - 1 - y;
                    if (adjacentToRoom(mirrorX, mirrorY))
                    {
                        dungeon[width - 1 - x, height - 1 - y] = true;
                        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.transform.position = new Vector3(width - x, 0, width - y);
                        obj.transform.parent = mazeObject.transform;
                    }
                }
                yield return new WaitForSeconds(.1f);
            }
        }
    }

    private Vector2 randomExistingNotSurrounded() {
        List<Vector2> possibleCoords = new List<Vector2>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (dungeon[i, j] && !Surrounded(i,j)) possibleCoords.Add(new Vector2(i, j));
            }
        }
        return possibleCoords[Random.Range(0, possibleCoords.Count)];
    }

    private bool Surrounded(int x, int y) {
        bool surrounded = true;
        if (x < width - 1 && !dungeon[x + 1, y]) surrounded = false;
        if (x > 0 && !dungeon[x - 1, y]) surrounded = false;
        if (y < height - 1 && !dungeon[x, y + 1]) surrounded = false;
        if (y > 0 && !dungeon[x, y - 1]) surrounded = false;
        return surrounded;
    }

    private List<Vector2> OpenNeighbors(int x, int y) {
        List<Vector2> possible = new List<Vector2>();
        if (x < width - 1 && !dungeon[x + 1, y]) possible.Add(new Vector2(x + 1, y));
        if (x > 0 && !dungeon[x - 1, y]) possible.Add(new Vector2(x - 1, y));
        if (y < height - 1 && !dungeon[x, y + 1]) possible.Add(new Vector2(x, y + 1));
        if (y > 0 && !dungeon[x, y - 1]) possible.Add(new Vector2(x, y - 1));
        return possible;
    }

    private bool adjacentToRoom(int x, int y)
    {
        bool adjacent = false;
        if (x < width - 1 && dungeon[x + 1, y]) adjacent = true;
        if (x > 0 && dungeon[x - 1, y]) adjacent = true;
        if (y < height - 1 && dungeon[x, y + 1]) adjacent = true;
        if (y > 0 && dungeon[x, y - 1]) adjacent = true;
        return adjacent;
    }
}
