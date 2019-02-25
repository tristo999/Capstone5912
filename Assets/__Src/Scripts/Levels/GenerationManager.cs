﻿using QuickGraph;
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
    public List<GameObject> roomPrefabs = new List<GameObject>();
    public List<DungeonRoom> rooms = new List<DungeonRoom>();

    public float roomSize;

    private bool[,] dungeon;
    private GameObject mazeObject;
    public int width;
    public int height;
    public int generationAttempts;

    public int maxRoomWidth;
    public int maxRoomHeight;
    public int minRooms;
    public int maxRooms;

    public bool debug;

    private AdjacencyGraph<DungeonRoom, Edge<DungeonRoom>> dungeonGraph;

    private void Update() {
        //if (debug && Input.GetKeyDown(KeyCode.S))
            //StartCoroutine(DebugStemmingMaze());
    }

    public void GenerateStemmingMazeGraph() {
        dungeonGraph = new AdjacencyGraph<DungeonRoom, Edge<DungeonRoom>>();

        DungeonRoom centerRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], Vector3.zero, Quaternion.identity).GetComponent<DungeonRoom>();
        centerRoom.DistanceFromCenter = 0;
        dungeonGraph.AddVertex(centerRoom);

        for (int i = 0; i < generationAttempts; i++) {
            IEnumerable<DungeonRoom> possibleRooms = dungeonGraph.Vertices.Where(v => dungeonGraph.OutEdges(v).Count() < 4);
            DungeonRoom chosenRoom = possibleRooms.ToArray()[Random.Range(0, possibleRooms.Count())];
            IEnumerable<Edge<DungeonRoom>> edges = dungeonGraph.OutEdges(chosenRoom);
            List<int> openEdges = new List<int>(new int[]{0, 1, 2, 3});
            foreach (Edge<DungeonRoom> edge in edges) {
                TaggedEdge<DungeonRoom, int> taggedEdge = (TaggedEdge<DungeonRoom, int>)edge;
                if (openEdges.Contains(taggedEdge.Tag)) {
                    openEdges.Remove(taggedEdge.Tag);
                }
            }
            int chosenEdge = openEdges[Random.Range(0, openEdges.Count)];
            Vector3 position = chosenRoom.transform.position;
            if (chosenEdge == 0) {
                position.z += roomSize;
            } else if (chosenEdge == 1) {
                position.x += roomSize;
            } else if (chosenEdge == 2) {
                position.z -= roomSize;
            } else {
                position.x -= roomSize;
            }
            DungeonRoom newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], position, Quaternion.identity).GetComponent<DungeonRoom>();

            var newEdge = new TaggedEdge<DungeonRoom, int>(chosenRoom, newRoom, chosenEdge);
            dungeonGraph.AddVerticesAndEdge(newEdge);
        }
        System.Func<Edge<DungeonRoom>, double> edgeCost = e => 1;
        TryFunc<DungeonRoom, IEnumerable<Edge<DungeonRoom>>> tryGetPaths = dungeonGraph.ShortestPathsDijkstra(edgeCost, centerRoom);
        foreach (DungeonRoom room in dungeonGraph.Vertices) {
            IEnumerable<Edge<DungeonRoom>> path;
            if (tryGetPaths(room, out path)) {
                room.DistanceFromCenter = path.Count();
            }
        }
    }

    public List<Vector3> SpawnPositions(int amt) {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < amt; i++) {
            positions.Add(dungeonGraph.Vertices.ToArray()[Random.Range(0, dungeonGraph.Vertices.Count())].transform.position + new Vector3(roomSize/2f, 1.5f, roomSize/2f));
        }
        return positions;
    }

    public void GenerateStemmingMaze() {
        if (mazeObject != null) {
            Destroy(mazeObject);
        }
        mazeObject = new GameObject();
        dungeon = new bool[width, height];
        dungeon[width / 2, height / 2] = true;
        GameObject obj = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], Vector3.zero, Quaternion.identity);
        DungeonRoom centerRoom = obj.GetComponent<DungeonRoom>();
        centerRoom.DistanceFromCenter = 0;
        rooms.Add(centerRoom);

        for (int i = 0; i < generationAttempts; i++) {
            Vector2 existingCell = randomExistingNotSurrounded();
            List<Vector2> possible = OpenNeighbors((int)existingCell.x, (int)existingCell.y);
            Vector2 newCell = possible[Random.Range(0, possible.Count)];
            dungeon[(int)newCell.x, (int)newCell.y] = true;
            int mirrorX = width - 1 - (int)newCell.x;
            int mirrorY = height - 1 - (int)newCell.y;
            Vector3 pos = new Vector3(newCell.x, 0, newCell.y) * roomSize - new Vector3(width / 2 * roomSize, 0, height / 2 * roomSize);
            GameObject newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], pos, Quaternion.identity);
            DungeonRoom room = newRoom.GetComponent<DungeonRoom>();
            rooms.Add(room);
            if (adjacentToRoom(mirrorX, mirrorY) && Random.Range(0.0f, 1.0f) > .05f) {
                dungeon[mirrorX, mirrorY] = true;
                pos = new Vector3(mirrorX, 0, mirrorY) * roomSize - new Vector3(width / 2 * roomSize, 0, height / 2 * roomSize); 
                newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], pos, Quaternion.identity);
                room = newRoom.GetComponent<DungeonRoom>();
                rooms.Add(room);
            }
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
        GameObject obj = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)]);
        obj.transform.parent = mazeObject.transform;
        obj.transform.position = new Vector3(width/2, 0, height/2) * roomSize;
        for (int i = 0; i < generationAttempts; i++)
        {
            int x = Random.Range(0, width-1);
            int y = Random.Range(0, height-1);
            if (!dungeon[x,y] && adjacentToRoom(x,y))
            {
                dungeon[x, y] = true;
                obj = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)]);
                obj.transform.position = new Vector3(x, 0, y) * roomSize;
                obj.transform.parent = mazeObject.transform;
                if (Random.Range(0.0f,1.0f) > 0.2f)
                {
                    int mirrorX = width - 1 - x;
                    int mirrorY = height - 1 - y;
                    if (adjacentToRoom(mirrorX, mirrorY))
                    {
                        dungeon[width - 1 - x, height - 1 - y] = true;
                        obj = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)]);
                        obj.transform.position = new Vector3(width - x, 0, width - y) * roomSize;
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
