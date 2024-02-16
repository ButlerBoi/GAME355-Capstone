using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public TileBase floorTile;      
    public TileBase wallTile;       
    public Tilemap floorTilemap;    
    public Tilemap wallTilemap;     
    public int gridSizeX = 10;      
    public int gridSizeY = 10;      
    public int numSteps = 1000;     

    private Vector3Int currentPosition; 

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        //1
        //generate floor loop
        for (int i = 0; i < numSteps; i++)
        {
            MoveWalker(); //move   
            CreateFloorTile(currentPosition); //spawn
        }

        //2
        //generate walls once floor is complete
        GenerateWalls();
    }

    
    void MoveWalker()
    {
        currentPosition += GetRandomDirection(); //move
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, gridSizeX - 1);   //stay within bounds
        currentPosition.y = Mathf.Clamp(currentPosition.y, 0, gridSizeY - 1);   
    }

    // random direction number
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
        }
        return Vector3Int.zero;
    }

 
    void CreateFloorTile(Vector3Int position)
    {
        // create floor no overlap
        if (!floorTilemap.HasTile(position))
        {
            floorTilemap.SetTile(position, floorTile);
        }
    }

 
    void GenerateWalls()
    {
        BoundsInt floorBounds = floorTilemap.cellBounds; //floor info

        // loop floor extended bounds for wall gen
        for (int x = floorBounds.xMin - 1; x <= floorBounds.xMax + 1; x++)
        {
            for (int y = floorBounds.yMin - 1; y <= floorBounds.yMax + 1; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // no overlap
                if (!floorTilemap.HasTile(position) && !wallTilemap.HasTile(position))
                {
                    bool adjacentToFloor = false; //set flag false

                    //loop search 3x3 grid around current floor tile
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            Vector3Int adjacentPosition = new Vector3Int(i, j, 0); //current adjacent

                            //if adjacent has floor tile
                            if (floorTilemap.HasTile(adjacentPosition))
                            {
                                adjacentToFloor = true; //set flag true exit inner loop
                                break;
                            }
                        }
                        if (adjacentToFloor) break; //if true exit outter loop for wall gen
                    }

                    //wall gen
                    if (adjacentToFloor)
                    {
                        wallTilemap.SetTile(position, wallTile);
                    }
                }
            }
        }
    }
}
