using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agrid : MonoBehaviour
{

    public LayerMask unWalkMask;
    public Vector2 gridWolrSize;
    public float nodeRadius;
    ANode[,] grid;

    float nodeDiameter;
    int gridX;
    int gridY;



    //public GameObject Tile_prefaps;
    //public int xGrid = 20;
    //public int zGrid = 20;
    //public Tile[,] map_Arr;
    //public int zeroToXGrid = 0;
    //public int zeroToZGrid = 0;


    private void Awake()
    {
        nodeDiameter = nodeRadius*2;
        gridX = Mathf.RoundToInt(gridWolrSize.x / nodeDiameter);
        gridY = Mathf.RoundToInt(gridWolrSize.y / nodeDiameter);
        CreateGrid();
    }

    void Start()
    {

    }

    private void CreateGrid()
    {
        grid = new ANode[gridX, gridY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWolrSize.x / 2 - Vector3.forward * gridWolrSize.y / 2;
        Vector3 worldPoint;
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool isWalkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unWalkMask));
                grid[x, y] = new ANode(isWalkable, worldPoint, x, y,null);
            }
        }
    }

    public ANode GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWolrSize.x / 2) / gridWolrSize.x;
        float percentY = (worldPosition.z + gridWolrSize.y / 2) / gridWolrSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridX - 1) * percentX);
        int y = Mathf.RoundToInt((gridY - 1) * percentY);

        return grid[x, y];

    }

    public List<ANode> GetNeighbour(ANode node)
    {
        List<ANode> neighbours = new List<ANode>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridX && checkY >= 0 && checkY < gridY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }



    public List<ANode> path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWolrSize.x, 1, gridWolrSize.y));
        if (grid != null)
        {
            foreach (var n in grid)
            {
                Gizmos.color = (n.isWalkable) ? Color.green : Color.red;
                Gizmos.DrawCube(n.world_Pos, Vector3.one * (nodeDiameter - .1f));

                if(path!= null)
                {
                    if(path.Contains(n))
                    {
                        Gizmos.color = Color.black;

                    }
                }
                    Gizmos.DrawSphere(n.world_Pos, nodeRadius / 2 - 0.1f);
            }
        }
    }
}
