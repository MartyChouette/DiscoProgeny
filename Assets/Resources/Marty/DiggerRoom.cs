using UnityEngine;
using System.Collections.Generic;

public class DiggerRoom : Room
{
    public float spawnProbability = 0.1f;
    public int maxDiggers = 50;
    public float baseDeathProbability = 0.05f;
    public float roomDiggerProbability = 0.2f;
    public int roomBranchLength = 5;

    public int extraDigIterations = 1000;

    private List<Vector2Int> mainDiggerPath;

    private class Digger
    {
        public int x, y;
        public bool isMain;
        public int branchSteps;

        public Digger(int x, int y, bool isMain, int branchSteps)
        {
            this.x = x;
            this.y = y;
            this.isMain = isMain;
            this.branchSteps = branchSteps;
        }
        public Vector2Int Position { get { return new Vector2Int(x, y); } }
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        // initialize the grid
        for (int r = 0; r < LevelGenerator.ROOM_HEIGHT; r++)
        {
            for (int c = 0; c < LevelGenerator.ROOM_WIDTH; c++)
            {
                indexGrid[c, r] = 1;
            }
        }

        // determine the start and exit nodes 
        Vector2Int startNode = Vector2Int.zero;
        Vector2Int exitNode = Vector2Int.zero;
        bool foundStart = false;
        foreach (Vector2Int exit in requiredExits.requiredExitLocations())
        {
            if (!foundStart)
            {
                startNode = exit;
                foundStart = true;
            }
            else
            {
                exitNode = exit;
            }
        }

        // run random walk to create a path
        RunRandomWalk(startNode, exitNode);
        // make path
        if (mainDiggerPath != null && mainDiggerPath.Count > 0)
        {
            foreach (Vector2Int pos in mainDiggerPath)
            {
                indexGrid[pos.x, pos.y] = 0;
            }
        }

        // dig rooms from random floor positions
        DigRooms();

        // spawn room tiles based on final grid
        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                int tileIndex = indexGrid[i, j];
                if (tileIndex == 0)
                    continue; // 0 represents a floor cell
                GameObject tileToSpawn;
                if (tileIndex < LevelGenerator.LOCAL_START_INDEX)
                {
                    tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex - 1];
                }
                else
                {
                    tileToSpawn = localTilePrefabs[tileIndex - LevelGenerator.LOCAL_START_INDEX];
                }
                Tile.spawnTile(tileToSpawn, transform, i, j);
            }
        }
    }

    // run multi-digger random walk until main digger reaches exit
    private void RunRandomWalk(Vector2Int startNode, Vector2Int exitNode)
    {
        List<Digger> diggers = new List<Digger>();

        // create main digger w infinity steps at the start
        Digger mainDigger = new Digger(startNode.x, startNode.y, true, -1);
        diggers.Add(mainDigger);
        mainDiggerPath = new List<Vector2Int>();
        mainDiggerPath.Add(new Vector2Int(startNode.x, startNode.y));

        bool mainDiggerReachedExit = false;

        // run until main digger reaches the exit
        while (!mainDiggerReachedExit)
        {
            // copy list of current diggers for safe iteration
            List<Digger> currentDiggers = new List<Digger>(diggers);

            foreach (Digger digger in currentDiggers)
            {
                // main digger is biased toward exit, others move randomly
                Vector2Int direction = digger.isMain ? ChooseBiasedDirection(digger, exitNode)
                                                    : ChooseRandomDirection();

                int newX = digger.x + direction.x;
                int newY = digger.y + direction.y;

                if (IsInBounds(newX, newY))
                {
                    // dig cell
                    indexGrid[newX, newY] = 0;

                    digger.x = newX;
                    digger.y = newY;

                    // record main digger path
                    if (digger.isMain)
                    {
                        mainDiggerPath.Add(new Vector2Int(newX, newY));
                    }

                    // main digger reaches the exit stop
                    if (digger.isMain && newX == exitNode.x && newY == exitNode.y)
                    {
                        mainDiggerReachedExit = true;
                        break;
                    }

                    // spawn a room branch digger
                    if (digger.isMain && diggers.Count < maxDiggers && Random.value < roomDiggerProbability)
                    {
                        Digger branchDigger = new Digger(newX, newY, false, roomBranchLength);
                        diggers.Add(branchDigger);
                    }

                    // spawn a new secondary digger
                    if (diggers.Count < maxDiggers && Random.value < spawnProbability)
                    {
                        Digger newDigger = new Digger(newX, newY, false, roomBranchLength);
                        diggers.Add(newDigger);
                    }
                }

                // decrease branchSteps
                if (!digger.isMain && digger.branchSteps > 0)
                {
                    digger.branchSteps--;
                    if (digger.branchSteps <= 0)
                    {
                        diggers.Remove(digger);
                    }
                }

                // remove diggers based on death probability
                if (!digger.isMain)
                {
                    float dynamicDeathProbability = baseDeathProbability * ((float)diggers.Count / maxDiggers);
                    if (Random.value < dynamicDeathProbability)
                    {
                        diggers.Remove(digger);
                    }

                }
            }
        }
        // clear all diggers 
        diggers.Clear();
    }

    private void DigRooms()
    {
        // carving from any position from main path
        List<Vector2Int> digList = new List<Vector2Int>(mainDiggerPath);

        for (int i = 0; i < extraDigIterations; i++)
        {
            // choose random cell
            Vector2Int source = digList[Random.Range(0, digList.Count)];
            Vector2Int direction = ChooseRandomDirection();

            int newX = source.x + direction.x;
            int newY = source.y + direction.y;

            if (IsInBounds(newX, newY))
            {
                // dig out cell
                indexGrid[newX, newY] = 0;
                // add cell to dig sources
                digList.Add(new Vector2Int(newX, newY));
            }
        }
    }

    //true if x,y within grid bounds
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < LevelGenerator.ROOM_WIDTH && y >= 0 && y < LevelGenerator.ROOM_HEIGHT;
    }

    // returns random direction
    private Vector2Int ChooseRandomDirection()
    {
        int r = Random.Range(0, 4);
        switch (r)
        {
            case 0: return new Vector2Int(1, 0); // right
            case 1: return new Vector2Int(-1, 0); // left
            case 2: return new Vector2Int(0, 1); // up
            case 3: return new Vector2Int(0, -1); // down
            default: return Vector2Int.zero;
        }
    }

    // choose direction toward exit using manhattan distance
    private Vector2Int ChooseBiasedDirection(Digger digger, Vector2Int target)
    {
        int currentDistance = Mathf.Abs(digger.x - target.x) + Mathf.Abs(digger.y - target.y);
        List<Vector2Int> directions = new List<Vector2Int>()
        {
            new Vector2Int(1, 0), // right
            new Vector2Int(-1, 0), // left
            new Vector2Int(0, 1), // up
            new Vector2Int(0, -1) // down
        };

        List<Vector2Int> possibleDirectionList = new List<Vector2Int>();

        foreach (Vector2Int d in directions)
        {
            int newX = digger.x + d.x;
            int newY = digger.y + d.y;

            if (!IsInBounds(newX, newY))
            {
                continue;
            }
            int newDistance = Mathf.Abs(newX - target.x) + Mathf.Abs(newY - target.y);

            if (newDistance < currentDistance)
            {
                possibleDirectionList.Add(d);
            }
        }

        if (possibleDirectionList.Count > 0)
        {
            return possibleDirectionList[Random.Range(0, possibleDirectionList.Count)];
        }

        return ChooseRandomDirection();
    }
}
