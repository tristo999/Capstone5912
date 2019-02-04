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


    public void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMaze(width, height);
        }
    }

    public void GenerateMaze(int width, int height)
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
            }
        }
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
