using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Vector2Int gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    public Vector2Int gridSize;

    Node[,] grid;

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        float nodeDiameter = nodeRadius * 2;
        int gridSizeX = gridSize.x;
        int gridSizeY = gridSize.y;

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = floorTilemap.transform.position - Vector3.right * floorTilemap.size.x / 2 - Vector3.up * floorTilemap.size.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);

                grid[x, y] = new Node(walkable, worldPoint, x, y); 
                grid[x, y] = gameObject.AddComponent<Node>(); 
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Check adjacent nodes in a 3x3 grid around the current node
        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0)
                    continue;

                int neighborX = node.gridX + xOffset;
                int neighborY = node.gridY + yOffset;

                if (neighborX >= 0 && neighborX < gridSize.x && neighborY >= 0 && neighborY < gridSize.y)
                {
                    neighbors.Add(grid[neighborX, neighborY]);
                }
            }
        }
        return neighbors;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(floorTilemap.transform.position, new Vector3(floorTilemap.size.x, floorTilemap.size.y, 1));

        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeRadius * 2 - 0.1f));
            }
        }
    }

    public Node WorldToNode(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x + gridSize.x / 2) / gridSize.x);
        float percentY = Mathf.Clamp01((worldPosition.y + gridSize.y / 2) / gridSize.y);

        int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

        return grid[x, y];
    }

    public Vector3 NodeToWorld(Node node)
    {
        return node.worldPosition;
    }
}