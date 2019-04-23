using MyBox;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationManager : BoltSingletonPrefab<GenerationManager>
{
    public List<GameObject> centerPrefabs = new List<GameObject>();

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
    [Range(0f, 1f)]
    public float wallKnockoutChance;
    public int amountWallsKnockout;

    private bool[,] dungeon;
    private DungeonRoom[,] vertices;
    private DungeonRoom centerRoom;
    private GameObject mazeObject;
    private GameObject batchingRoot;
    public int width;
    public int height;
    public int generationAttempts;
    public int spawnRoomFadeRange;
    public float perlinScale = 1f;
    [Space(20)]
    public AnimationCurve rarityCurve;


    [HideInInspector]
    public List<DungeonRoom> spawnRooms;
    private int maxDist;

    [Tooltip("This does nothing right now. Sorry... <3 David")]
    public bool debug;

    public AdjacencyGraph<DungeonRoom, Edge<DungeonRoom>> dungeonGraph;

    private DungeonRoom northNeighbor;
    private DungeonRoom northNeighbor2;
    private DungeonRoom southNeighbor;
    private DungeonRoom southNeighbor2;
    private DungeonRoom eastNeighbor;
    private DungeonRoom eastNeighbor2;
    private DungeonRoom westNeighber;
    private DungeonRoom westNeighbor2;

    private List<BoltEntity> enemies;

    public void DoGeneration(int playerCount) {
        batchingRoot = new GameObject();

        using (new TimeTest("Initial Generation"))
            GenerateStemmingMazeGraph();
        using (new TimeTest("Calculating Room Distances", true))
            CalculateRoomDistances();
        using (new TimeTest("Setting Spawn Rooms", true))
            spawnRooms = GetFirstEquidistantRooms(dungeonGraph.Vertices.Where(r => r.DistanceFromCenter >= maxDist - 2), playerCount).ToList();
        //using (new TimeTest("Adding Perlin Noise", true))
            //AddPerlinNoise();
        using (new TimeTest("Generating Danger Ratings", true))
            GenerateDangerRatings();
        using (new TimeTest("Populating Rooms", true)) {
            PopulateTags();
            SpawnDroppedItems();
            PopuplateChests();
        }
    }

    public void BumpEnemies() {
        foreach (BoltEntity enemy in enemies) {
            IEnemyState eState;
            if (enemy.TryFindState(out eState)) {
                Debug.Log("Attempting to fix enemy position for clients.");
                enemy.transform.position += enemy.transform.forward;
            }
        }
    }

    private void CalculateRoomDistances() {
        Queue<DungeonRoom> roomQueue = new Queue<DungeonRoom>();
        roomQueue.Enqueue(vertices[width / 2, height / 2]);
        roomQueue.Enqueue(vertices[width / 2 - 1, height / 2]);
        roomQueue.Enqueue(vertices[width / 2 - 1, height / 2 - 1]);
        roomQueue.Enqueue(vertices[width / 2, height / 2 - 1]);

        while (roomQueue.Count > 0) {
            DungeonRoom currentRoom = roomQueue.Dequeue();
            foreach (Edge<DungeonRoom> edge in dungeonGraph.OutEdges(currentRoom)) {
                if (edge.Target.DistanceFromCenter == -1) {
                    edge.Target.DistanceFromCenter = edge.Source.DistanceFromCenter + 1;
                    edge.Target.state.DistanceFromCenter = edge.Source.DistanceFromCenter + 1;
                    if (edge.Target.DistanceFromCenter > maxDist)
                        maxDist = edge.Target.DistanceFromCenter;
                    roomQueue.Enqueue(edge.Target);
                }
            }
        }
    }

    private void AddPerlinNoise() {
        float seed = Random.Range(-1000f, 1000f);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (dungeon[x, y] && Mathf.PerlinNoise(perlinScale * (seed + x), perlinScale * (seed + y)) < 0.0025f) {
                    dungeonGraph.RemoveVertex(vertices[x, y]);
                    Destroy(vertices[x, y].gameObject);
                }
            }
        }
    }

    private void GenerateDangerRatings() {
        foreach (DungeonRoom room in dungeonGraph.Vertices) {
            int distanceFromEdge = maxDist - room.DistanceFromCenter;
            if (distanceFromEdge > room.DistanceFromCenter) {
                room.DangerRating = Mathf.Pow((distanceFromEdge - room.DistanceFromCenter) / (float)maxDist, 1);
            } else {
                room.DangerRating = Mathf.Pow((room.DistanceFromCenter - distanceFromEdge) / (float)maxDist, 1);
            }
            //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = room.transform.position + new Vector3(15f, 30f, 15f);
            //cube.transform.localScale = new Vector3(30f, 1f, 30f);
            //cube.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.red, room.DangerRating);
        }
    }

    private IEnumerable<DungeonRoom> GetFirstEquidistantRooms(IEnumerable<DungeonRoom> rooms, int amt) {
        foreach (DungeonRoom room in rooms) {
            Dictionary<int, List<DungeonRoom>> possibleRoomSets = new Dictionary<int, List<DungeonRoom>>();
            System.Func<Edge<DungeonRoom>, double> edgeWeight = e => 1;
            TryFunc<DungeonRoom, IEnumerable<Edge<DungeonRoom>>> tryDijkstra = dungeonGraph.ShortestPathsDijkstra(edgeWeight, room);
            foreach (DungeonRoom otherRoom in rooms) {
                IEnumerable<Edge<DungeonRoom>> edges;
                if (tryDijkstra(otherRoom, out edges)) {
                    int distance = edges.Count();
                    if (possibleRoomSets.ContainsKey(distance) && possibleRoomSets[distance].Count < amt) {
                        possibleRoomSets[distance].Add(otherRoom);
                    } else if (!possibleRoomSets.ContainsKey(distance)) {
                        possibleRoomSets[distance] = new List<DungeonRoom>();
                        possibleRoomSets[distance].Add(otherRoom);
                    }
                }
            }
            var validRooms = possibleRoomSets.FirstOrDefault(r => r.Value.Count == amt);
            if (validRooms.Value != null) {
                return validRooms.Value;
            }
        }
        List<DungeonRoom> ranRooms = new List<DungeonRoom>();
        List<DungeonRoom> posRooms = rooms.ToList();
        for (int i = 0; i < amt; i++) {
            DungeonRoom selected = posRooms[Random.Range(0, posRooms.Count)];
            ranRooms.Add(selected);
            posRooms.Remove(selected);
        }
        return ranRooms;
    }

    public void GenerateStemmingMazeGraph() {
        dungeonGraph = new AdjacencyGraph<DungeonRoom, Edge<DungeonRoom>>();
        GenerateStemmingMaze();

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (i == width / 2 && j == height / 2) {
                    centerRoom = vertices[i, j];
                }
                if (vertices[i, j]) {
                    if (i < width && vertices[i + 1, j]) {
                        Edge<DungeonRoom> edge = new Edge<DungeonRoom>(vertices[i, j], vertices[i + 1, j]);
                        if (edge.Source.DistanceFromCenter == 0 && edge.Target.DistanceFromCenter == 0) {
                            edge.Source.state.EastWall = (int)DungeonRoom.WallState.Open;
                            edge.Target.state.WestWall = (int)DungeonRoom.WallState.Open;
                        } else {
                            edge.Source.state.EastWall = (int)DungeonRoom.WallState.Door;
                            edge.Target.state.WestWall = (int)DungeonRoom.WallState.Door;
                        }
                        dungeonGraph.AddVerticesAndEdge(edge);
                        dungeonGraph.AddEdge(new Edge<DungeonRoom>(edge.Target, edge.Source));
                    }
                    if (j < height && vertices[i, j + 1]) {
                        Edge<DungeonRoom> edge = new Edge<DungeonRoom>(vertices[i, j], vertices[i, j + 1]);
                        if (edge.Source.DistanceFromCenter == 0 && edge.Target.DistanceFromCenter == 0) {
                            edge.Source.state.NorthWall = (int)DungeonRoom.WallState.Open;
                            edge.Target.state.SouthWall = (int)DungeonRoom.WallState.Open;
                        } else {
                            edge.Source.state.NorthWall = (int)DungeonRoom.WallState.Door;
                            edge.Target.state.SouthWall = (int)DungeonRoom.WallState.Door;
                        }
                        dungeonGraph.AddVerticesAndEdge(edge);
                        dungeonGraph.AddEdge(new Edge<DungeonRoom>(edge.Target, edge.Source));
                    }
                }
            }
        }
    }


    public Vector3 GetSpawnPos(int player) {
        return spawnRooms[player].transform.position + new Vector3(roomSize / 2, 1, roomSize / 2);
    }

    public void PopulateTags() {
        SpawnTagFromList("CarpetSpawner", carpetObjects);
        SpawnTagFromList("CabinetClutter", cabinetObjects);
        enemies = SpawnTagFromList("EnemySpawn", enemyEntities).ToList();
        SpawnTagFromList("ChestSpawn", chestEntities);
        SpawnTagFromList("TableClutter", tableObjects);
        SpawnTagFromList("GroundClutter", groundObjects);
    }

    private IEnumerable<BoltEntity> SpawnTagFromList(string tag, List<GameObject> list) {
        List<BoltEntity> spawnedFromTag = new List<BoltEntity>();
        foreach (GameObject spawn in GameObject.FindGameObjectsWithTag(tag)) {
            if (Random.Range(0f, 1f) <= spawn.GetComponent<SpawnChance>().Chance) {
                BoltEntity spawned = BoltNetwork.Instantiate(list[Random.Range(0, list.Count)], spawn.transform.position, spawn.transform.rotation);
                spawnedFromTag.Add(spawned);
                DangerRating dr = spawned.GetComponent<DangerRating>();
                if (dr) {
                    dr.rating = spawn.GetComponentInParent<DungeonRoom>().DangerRating;
                }
                foreach (GameObject child in FindChildrenWithTag(spawn, "ChildClutter")) {
                    if (Random.Range(0f, 1f) <= child.GetComponent<SpawnChance>().Chance) {
                        BoltNetwork.Instantiate(childClutterObjects[Random.Range(0, childClutterObjects.Count)], child.transform.position, child.transform.rotation);
                        BoltNetwork.Destroy(child);
                    }
                }
            }
            BoltNetwork.Destroy(spawn);
        }
        return spawnedFromTag;
    }

    private void SpawnDroppedItems() {
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("DroppedItemSpawn")) {
            DroppedItemSpawner spawnerScript = spawner.GetComponent<DroppedItemSpawner>();
            if (Random.Range(0f, 1f) <= spawnerScript.spawnChance) {
                ItemManager.Instance.SpawnItemFromRarity(spawnerScript.preferredRarity, spawner.transform.position);
            }
            BoltNetwork.Destroy(spawner);
        }
    }

    private void PopuplateChests() {
        foreach (NormalChest chest in Resources.FindObjectsOfTypeAll<NormalChest>()) {
            float rating = chest.GetComponent<DangerRating>().rating;
            float randomValWeighted = Mathf.Pow(Random.value, 1 - rating * 0.25f); // rating of 0 = linear, rating of 1 = slight root function
            chest.ContainedItem = ItemManager.Instance.ItemFromDangerRating(rarityCurve.Evaluate(randomValWeighted));
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
        DungeonRoom obj = BoltNetwork.Instantiate(centerPrefabs[1], Vector3.zero, Quaternion.identity).GetComponent<DungeonRoom>();
        centerRoom = obj;
        vertices[width / 2, height / 2] = obj;
        vertices[width / 2 - 1, height / 2] = BoltNetwork.Instantiate(centerPrefabs[0], new Vector3(-30, 0, 0), Quaternion.identity).GetComponent<DungeonRoom>();
        vertices[width / 2 - 1, height / 2 - 1] = BoltNetwork.Instantiate(centerPrefabs[2], new Vector3(-30, 0, -30), Quaternion.identity).GetComponent<DungeonRoom>();
        vertices[width / 2, height / 2 - 1] = BoltNetwork.Instantiate(centerPrefabs[3], new Vector3(0, 0, -30), Quaternion.identity).GetComponent<DungeonRoom>();

        vertices[width / 2 - 1, height / 2].state.EastWall = (int)DungeonRoom.WallState.Open;
        vertices[width / 2 - 1, height / 2].state.SouthWall = (int)DungeonRoom.WallState.Open;
        vertices[width / 2, height / 2].state.WestWall = (int)DungeonRoom.WallState.Open;
        vertices[width / 2, height / 2].state.SouthWall = (int)DungeonRoom.WallState.Open;
        vertices[width / 2 - 1, height / 2 - 1].state.EastWall = (int)DungeonRoom.WallState.Open;
        vertices[width / 2 - 1, height / 2 - 1].state.NorthWall = (int)DungeonRoom.WallState.Open;
        vertices[width / 2, height / 2 - 1].state.WestWall = (int)DungeonRoom.WallState.Open;
        vertices[width / 2, height / 2 - 1].state.NorthWall = (int)DungeonRoom.WallState.Open;

        vertices[width / 2, height / 2].DistanceFromCenter = 0;
        vertices[width / 2 - 1, height / 2].DistanceFromCenter = 0;
        vertices[width / 2 - 1, height / 2 - 1].DistanceFromCenter = 0;
        vertices[width / 2, height / 2 - 1].DistanceFromCenter = 0;
        vertices[width / 2, height / 2].state.DistanceFromCenter = 0;
        vertices[width / 2 - 1, height / 2].state.DistanceFromCenter = 0;
        vertices[width / 2 - 1, height / 2 - 1].state.DistanceFromCenter = 0;
        vertices[width / 2, height / 2 - 1].state.DistanceFromCenter = 0;

        for (int i = 0; i < generationAttempts; i++) {
            Vector2 existingCell = randomExistingNotSurrounded();
            List<Vector2> possible = OpenNeighbors((int)existingCell.x, (int)existingCell.y);
            Vector2 newCell = possible[Random.Range(0, possible.Count)];
            //vertices[(int)newCell.x, (int)newCell.y] = true;
            int mirrorX = width - 1 - (int)newCell.x;
            int mirrorY = height - 1 - (int)newCell.y;
            Vector3 pos = new Vector3(newCell.x, 0, newCell.y) * roomSize - new Vector3(width / 2 * roomSize, 0, height / 2 * roomSize);
            GameObject newRoom;
            if (Random.Range(0f, 1f) < .075f) {
                newRoom = BoltNetwork.Instantiate(specialRooms[Random.Range(0, specialRooms.Count)], pos, Quaternion.identity);
            } else {
                newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], pos, Quaternion.identity);
            }
            DungeonRoom room = newRoom.GetComponent<DungeonRoom>();
            vertices[(int)newCell.x, (int)newCell.y] = room;
            if (!vertices[mirrorX, mirrorY] && adjacentToRoom(mirrorX, mirrorY) && Random.Range(0.0f, 1.0f) > .05f) {
                //vertices[mirrorX, mirrorY] = true;
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
                if (vertices[i, j] && !Surrounded(i, j)) possibleCoords.Add(new Vector2(i, j));
            }
        }
        return possibleCoords[Random.Range(0, possibleCoords.Count)];
    }

    private bool Surrounded(int x, int y) {
        bool surrounded = true;
        if (x < width - 1 && !vertices[x + 1, y]) surrounded = false;
        if (x > 0 && !vertices[x - 1, y]) surrounded = false;
        if (y < height - 1 && !vertices[x, y + 1]) surrounded = false;
        if (y > 0 && !vertices[x, y - 1]) surrounded = false;
        return surrounded;
    }

    private List<Vector2> OpenNeighbors(int x, int y) {
        List<Vector2> possible = new List<Vector2>();
        if (x < width - 1 && !vertices[x + 1, y]) possible.Add(new Vector2(x + 1, y));
        if (x > 0 && !vertices[x - 1, y]) possible.Add(new Vector2(x - 1, y));
        if (y < height - 1 && !vertices[x, y + 1]) possible.Add(new Vector2(x, y + 1));
        if (y > 0 && !vertices[x, y - 1]) possible.Add(new Vector2(x, y - 1));
        return possible;
    }

    private bool adjacentToRoom(int x, int y) {
        bool adjacent = false;
        if (x < width - 1 && vertices[x + 1, y]) adjacent = true;
        if (x > 0 && vertices[x - 1, y]) adjacent = true;
        if (y < height - 1 && vertices[x, y + 1]) adjacent = true;
        if (y > 0 && vertices[x, y - 1]) adjacent = true;
        return adjacent;
    }

    private void MergeVertices(DungeonRoom to, DungeonRoom from) {
        IEnumerable<Edge<DungeonRoom>> edges = dungeonGraph.OutEdges(from);
        foreach (Edge<DungeonRoom> edge in edges) {
            Edge<DungeonRoom> newEdge = new Edge<DungeonRoom>(to, edge.Target);
            dungeonGraph.AddVerticesAndEdge(newEdge);
        }
        dungeonGraph.RemoveVertex(from);
        BoltNetwork.Destroy(from.gameObject);
    }

    public void DestroyNeighborWalls(DungeonRoom room) {
        float halfRoom = roomSize / 2f;
        Debug.Log("Destroying neighbor walls");
        foreach (Edge<DungeonRoom> edge in dungeonGraph.OutEdges(room)) {
            if (edge.Target.transform.position.x - edge.Source.transform.position.x > 5) {
                // room to the right 
                edge.Target.state.WestWall = (int)DungeonRoom.WallState.Destroyed;
            } else if (edge.Target.transform.position.x - edge.Source.transform.position.x < -5) {
                // room to the left
                edge.Target.state.EastWall = (int)DungeonRoom.WallState.Destroyed;
            } else if (edge.Target.transform.position.y - edge.Source.transform.position.y > 5) {
                // room above
                edge.Target.state.SouthWall = (int)DungeonRoom.WallState.Destroyed;
            } else if (edge.Target.transform.position.y - edge.Source.transform.position.y < -5) {
                // room below
                edge.Target.state.NorthWall = (int)DungeonRoom.WallState.Destroyed;
            }
        }
    }
}
