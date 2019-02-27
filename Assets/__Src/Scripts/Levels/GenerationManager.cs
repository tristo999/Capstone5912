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
        GenerateStemmingMaze();

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
                    }
                    if (j < height && dungeon[i,j+1]) {
                        Edge<DungeonRoom> edge = new Edge<DungeonRoom>(vertices[i, j], vertices[i, j + 1]);
                        edge.Source.state.NorthWall = (int)DungeonRoom.WallState.Open;
                        edge.Target.state.SouthWall = (int)DungeonRoom.WallState.Open;
                        dungeonGraph.AddVerticesAndEdge(edge);
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
            GameObject newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], pos, Quaternion.identity);
            DungeonRoom room = newRoom.GetComponent<DungeonRoom>();
            vertices[(int)newCell.x, (int)newCell.y] = room;
            if (!dungeon[mirrorX, mirrorY] && adjacentToRoom(mirrorX, mirrorY) && Random.Range(0.0f, 1.0f) > .05f) {
                dungeon[mirrorX, mirrorY] = true;
                pos = new Vector3(mirrorX, 0, mirrorY) * roomSize - new Vector3(width / 2 * roomSize, 0, height / 2 * roomSize); 
                newRoom = BoltNetwork.Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], pos, Quaternion.identity);
                room = newRoom.GetComponent<DungeonRoom>();
                vertices[mirrorX, mirrorY] = room;
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