using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GameManager : MonoBehaviour
{
    public TileBase floorTile;
    public TileBase wallTile;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public GameObject playerPrefab;
    public GameObject spawnerPrefab;
    public GameObject chestPrefab; 
    public int gridSizeX = 100;
    public int gridSizeY = 100;
    public int walkerSteps = 100;
    public int numRooms = 3;
    public int minHallwayLength = 5;
    public int maxHallwayLength = 10;
    public float chestSpawnChance = 0.5f;


    private Vector3Int currentPosition;
    private List<Vector3Int> roomPositions = new List<Vector3Int>();
    private List<Vector3Int> hallwayPositions = new List<Vector3Int>();

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        Vector3Int previousRoomPosition = Vector3Int.zero;
        Vector3Int playerSpawnPosition = Vector3Int.zero;

        // Generate the first room
        GenerateRoom(previousRoomPosition);
        playerSpawnPosition = roomPositions[roomPositions.Count - 1]; // Update player spawn position

        // Spawn the player GameObject at the designated spawn point
        playerPrefab.transform.position = floorTilemap.GetCellCenterWorld(playerSpawnPosition);

        for (int i = 0; i < numRooms; i++)
        {
            GenerateRoom(previousRoomPosition);
            if (i < numRooms - 1) // not last room (no hallway for the last room)
            {
                WalkHallway(); 

                previousRoomPosition = hallwayPositions[hallwayPositions.Count - 1];

                Vector3Int currentRoomCenter = roomPositions[roomPositions.Count - 1];

                if (i > 0)
                {
                    Vector3Int spawnerSpawnPosition = FindSuitableSpawnerPositionInRoom(currentRoomCenter);
                    Instantiate(spawnerPrefab, floorTilemap.GetCellCenterWorld(spawnerSpawnPosition), Quaternion.identity);
                }
                if (Random.value < chestSpawnChance)
                {
                    Vector3Int chestSpawnPosition = currentRoomCenter;
                    Instantiate(chestPrefab, floorTilemap.GetCellCenterWorld(chestSpawnPosition), Quaternion.identity);
                }
            }
        }
        GenerateWalls();
    }

    Vector3Int FindSuitableSpawnerPositionInRoom(Vector3Int roomCenter)
    {
        Vector3Int bestPosition = roomCenter; 
        int bestScore = int.MinValue; 

        foreach (Vector3Int tilePosition in roomPositions)
        {
            // Calculate the distance from the room center to the current tile
            int distanceToCenter = Mathf.Abs(tilePosition.x - roomCenter.x) + Mathf.Abs(tilePosition.y - roomCenter.y);

            // Check if the tile meets the criteria for a suitable spawn position
            if (IsSuitableSpawnPosition(tilePosition))
            {
                // Calculate a score for the tile based on its distance to the center
                int score = CalculateScore(distanceToCenter);

                // Update the best position if the current tile has a higher score
                if (score > bestScore)
                {
                    bestPosition = tilePosition;
                    bestScore = score;
                }
            }
        }

        return bestPosition;
    }

    bool IsSuitableSpawnPosition(Vector3Int position)
    {
        // Check if the position is within the bounds of the grid
        if (position.x < 0 || position.x >= gridSizeX || position.y < 0 || position.y >= gridSizeY)
        {
            return false;
        }

        // Check if the position is not obstructed by walls
        if (wallTilemap.HasTile(position))
        {
            return false;
        }

        return true; // Position meets all criteria
    }

    int CalculateScore(int distanceToCenter)
    {
        return Mathf.Max(0, 1 - distanceToCenter);
    }

    void GenerateRoom(Vector3Int previousRoomPosition)
    {
        currentPosition = previousRoomPosition;
        for (int step = 0; step < walkerSteps; step++)
        {
            MoveWalker();
            CreateFloorTile(currentPosition);

            Vector3Int adjacentPosition = currentPosition + GetRandomDirection();
            CreateFloorTile(adjacentPosition);
        }
        roomPositions.Add(currentPosition);
    }

    Vector3Int GetRandomDirection()
    {
        int dir = Random.Range(0, 4);
        switch (dir)
        {
            case 0:
                return Vector3Int.up;
            case 1:
                return Vector3Int.down;
            case 2:
                return Vector3Int.left;
            case 3:
                return Vector3Int.right;
            default:
                return Vector3Int.zero;
        }
    }

    void WalkHallway()
    {
        Vector3Int start = roomPositions[roomPositions.Count - 1]; // Start from the last generated room
        Vector3Int direction = GetRandomDirection();

        int hallwayLength = Random.Range(minHallwayLength, maxHallwayLength + 1); // Adjusted to include maxHallwayLength
        Vector3Int end = start + direction * hallwayLength;

        CreateHallway(start, end, direction);
    }

    void CreateHallway(Vector3Int start, Vector3Int end, Vector3Int direction)
    {
        Vector3Int perpendicularDirection = new Vector3Int(-direction.y, direction.x, 0); // Perpendicular to hallway direction

        int width = 2; // Width of the hallway

        for (int i = 0; i < width; i++) // Loop for the width of the hallway
        {
            Vector3Int currentStart = start + perpendicularDirection * i;
            Vector3Int currentEnd = end + perpendicularDirection * i;

            Vector3Int current = currentStart;

            while (current != currentEnd)
            {
                if (current.x >= 0 && current.x < gridSizeX && current.y >= 0 && current.y < gridSizeY)
                {
                    if (!roomPositions.Contains(current))
                    {
                        floorTilemap.SetTile(current, floorTile);
                        hallwayPositions.Add(current);
                    }
                }
                current += direction;
            }
        }
    }

    void MoveWalker()
    {
        Vector3Int direction = GetRandomDirection();
        currentPosition += direction;
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, gridSizeX - 1);
        currentPosition.y = Mathf.Clamp(currentPosition.y, 0, gridSizeY - 1);
    }

    void CreateFloorTile(Vector3Int position)
    {
        if (!floorTilemap.HasTile(position))
        {
            floorTilemap.SetTile(position, floorTile);
        }
    }

    void GenerateWalls()
    {
        BoundsInt floorBounds = floorTilemap.cellBounds;

        for (int x = floorBounds.xMin - 1; x <= floorBounds.xMax + 1; x++)
        {
            for (int y = floorBounds.yMin - 1; y <= floorBounds.yMax + 1; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if (!floorTilemap.HasTile(position) && !wallTilemap.HasTile(position))
                {
                    bool adjacentToFloor = false;

                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            Vector3Int adjacentPosition = new Vector3Int(i, j, 0);

                            if (floorTilemap.HasTile(adjacentPosition))
                            {
                                adjacentToFloor = true;
                                break;
                            }
                        }
                        if (adjacentToFloor) break;
                    }

                    if (adjacentToFloor)
                    {
                        position.z = 0;
                        wallTilemap.SetTile(position, wallTile);
                    }
                }
            }
        }
    }
}

