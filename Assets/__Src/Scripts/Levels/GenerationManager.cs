using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct DungeonCell
{
    public bool NorthExit { get; set; }
    public bool SouthExit { get; set; }
    public bool EastExit { get; set; }
    public bool WestExit { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Generation { get; set; }
}

public class GenerationManager : BoltSingletonPrefab<GenerationManager>
{
    public List<DungeonCell> possibleCells = new List<DungeonCell>();
    [Header("Room Generation Prefabs")]
    public List<GameObject> roomPrefabs = new List<GameObject>();
    public List<GameObject> specialRooms = new List<GameObject>();

    [Tooltip("CarpetSpawner tag.")]
    public List<GameObject> carpetObjects = new List<GameObject>();
    [Tooltip("CabinetClutter tag.")]
    public List<GameObject> cabinetObjects = new List<GameObject>();
    [Tooltip("EnemySpawn tag.")]
    public List<GameObject> enemyEntities = new List<GameObject>();
    [Tooltip("ChestSpawn tag.")]
    public List<GameObject> chestEntities = new List<GameObject>();
    [Tooltip("TableClutter tag.")]
    public List<GameObject> tableObjects = new List<GameObject>();
    [Tooltip("GroundClutter tag.")]
    public List<GameObject> groundObjects = new List<GameObject>();
    [Tooltip("ChildClutter tag.")]
    public List<GameObject> childClutterObjects = new List<GameObject>();
    [Space(20)]

    [Header("Generation Settings")]
    public float roomSize;
    [Range(0f,1f)]
    public float wallKnockoutChance;
    public int amountWallsKnockout;

    private bool[,] dungeon;
    private DungeonRoom[,] vertices;
    private DungeonRoom centerRoom;
    private GameObject mazeObject;
    public int width;
    public int height;
    public int generationAttempts;
    [Space(20)]

    [Tooltip("This does nothing right now. Sorry... <3 David")]
    public bool debug;

    public AdjacencyGraph<DungeonRoom, Edge<DungeonRoom>> dungeonGraph;

    public void GenerateStemmingMazeGraph() {
        dungeonGraph = new AdjacencyGraph<DungeonRoom, Edge<DungeonRoom>>();
        GenerateStemmingMaze();
        PopulateTags();

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (i == width/2 && j == height/2) {
                    centerRoom = vertices[i, j];
                }
                if (dungeon[i, j]) {
                    if (i < width && dungeon[i+1,j]) {
                        Edge<DungeonRoom> edge = new Edge<DungeonRoom>(vertices[i, j], vertices[i + 1, j]);
                        edge.Source.state.EastWall = (int)DungeonRoom.WallState.Open;
                        edge.Target.state.WestWall = (int)DungeonRoom.WallState.Open;
                        dungeonGraph.AddVerticesAndEdge(edge);
                        dungeonGraph.AddEdge(new Edge<DungeonRoom>(edge.Target, edge.Source));
                    }
                    if (j < height && dungeon[i,j+1]) {
                        Edge<DungeonRoom> edge = new Edge<DungeonRoom>(vertices[i, j], vertices[i, j + 1]);
                        edge.Source.state.NorthWall = (int)DungeonRoom.WallState.Open;
                        edge.Target.state.SouthWall = (int)DungeonRoom.WallState.Open;
                        dungeonGraph.AddVerticesAndEdge(edge);
                        dungeonGraph.AddEdge(new Edge<DungeonRoom>(edge.Target, edge.Source));
                    }
                }
            }
        }

        System.Func<Edge<DungeonRoom>, double> edgeWeight = e => 1;
        TryFunc<DungeonRoom, IEnumerable<Edge<DungeonRoom>>> tryDijkstra = dungeonGraph.ShortestPathsDijkstra<DungeonRoom, Edge<DungeonRoom>>(edgeWeight, centerRoom);
        foreach (DungeonRoom vertex in dungeonGraph.Vertices) {
            IEnumerable<Edge<DungeonRoom>> edges;
            if (tryDijkstra(vertex, out edges)) {
                vertex.DistanceFromCenter = edges.Count();
            } else if (vertex != centerRoom) {
                Debug.Log("Path doesn't exist to room!?");
            }
        }
    }

    public List<Vector3> SpawnPositions(int amt) {
        List<Vector3> positions = dungeonGraph.Vertices.OrderByDescending(r => r.DistanceFromCenter).Take(amt).Select(r => r.transform.position + new Vector3(15, 0, 15)).ToList();
        return positions;
    }

    public void PopulateTags() {
        SpawnTagFromList("CarpetSpawner", carpetObjects);
        SpawnTagFromList("CabinetClutter", cabinetObjects);
        SpawnTagFromList("EnemySpawn", enemyEntities);
        SpawnTagFromList("ChestSpawn", chestEntities);
        SpawnTagFromList("TableClutter", tableObjects);
        SpawnTagFromList("GroundClutter", groundObjects);
        SpawnDroppedItems();
    }

    private void SpawnTagFromList(string tag, List<GameObject> list) {
        foreach (GameObject spawn in GameObject.FindGameObjectsWithTag(tag)) {
            if (Random.Range(0f, 1f) <= spawn.GetComponent<SpawnChance>().Chance) {
                BoltNetwork.Instantiate(list[Random.Range(0, list.Count)], spawn.transform.position, spawn.transform.rotation);
                foreach (GameObject child in FindChildrenWithTag(spawn, "ChildClutter")) {
                    if (Random.Range(0f, 1f) <= child.GetComponent<SpawnChance>().Chance) {
                        BoltNetwork.Instantiate(childClutterObjects[Random.Range(0, childClutterObjects.Count)], child.transform.position, child.transform.rotation);
                        BoltNetwork.Destroy(child);
                    }
                }
            }
            BoltNetwork.Destroy(spawn);
        }
    }

    private void SpawnDroppedItems() {
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("DroppedItemSpawn")) {
            DroppedItemSpawner spawnerScript = spawner.GetComponent<DroppedItemSpawner>();
            if (Random.Range(0f,1f) <= spawnerScript.spawnChance) {
                ItemManager.Instance.SpawnItemFromRarity(spawnerScript.preferredRarity, spawner.transform.position);
            }
            BoltNetwork.Destroy(spawner);
        }
    }

    public GameObject[] FindChildrenWithTag(GameObject parent, string tag) {
        List<GameObject> children = new List<GameObject>();
        Transform t = parent.transform;
        foreach (Transform tr in t) {
            if (tr.tag == tag) {
                children.Add(tr.gameObject);
            }
            children.AddRange(FindChildrenWithTag(tr.gameObject, tag));
        }
        return children.ToArray();
    }

    public void GenerateStemmingMaze() {
        if (mazeObject != null) {
            Destroy(mazeObject);
        }
        //mazeObject = new GameObject();
        dungeon = new bool[width, height];
        dungeon[width / 2, height / 2] = true;
        vertices = new DungeonRoom[width, height];
        DungeonRoom obj = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], Vector3.zero, Quaternion.identity).GetComponent<DungeonRoom>();
        centerRoom = obj;
        obj.DistanceFromCenter = 0;
        vertices[width / 2, height / 2] = obj;

        for (int i = 0; i < generationAttempts; i++) {
            Vector2 existingCell = randomExistingNotSurrounded();
            List<Vector2> possible = OpenNeighbors((int)existingCell.x, (int)existingCell.y);
            Vector2 newCell = possible[Random.Range(0, possible.Count)];
            dungeon[(int)newCell.x, (int)newCell.y] = true;
            int mirrorX = width - 1 - (int)newCell.x;
            int mirrorY = height - 1 - (int)newCell.y;
            Vector3 pos = new Vector3(newCell.x, 0, newCell.y) * roomSize - new Vector3(width / 2 * roomSize, 0, height / 2 * roomSize);
            GameObject newRoom;
            if (Random.Range(0f,1f) < .075f) {
                newRoom = BoltNetwork.Instantiate(specialRooms[Random.Range(0, specialRooms.Count)], pos, Quaternion.identity);
            } else {
                newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], pos, Quaternion.identity);
            }
            DungeonRoom room = newRoom.GetComponent<DungeonRoom>();
            vertices[(int)newCell.x, (int)newCell.y] = room;
            if (!dungeon[mirrorX, mirrorY] && adjacentToRoom(mirrorX, mirrorY) && Random.Range(0.0f, 1.0f) > .05f) {
                dungeon[mirrorX, mirrorY] = true;
                pos = new Vector3(mirrorX, 0, mirrorY) * roomSize - new Vector3(width / 2 * roomSize, 0, height / 2 * roomSize);
                if (Random.Range(0f, 1f) < .075f) {
                    newRoom = BoltNetwork.Instantiate(specialRooms[Random.Range(0, specialRooms.Count)], pos, Quaternion.identity);
                } else {
                    newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], pos, Quaternion.identity);
                }
                room = newRoom.GetComponent<DungeonRoom>();
                vertices[mirrorX, mirrorY] = room;
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
