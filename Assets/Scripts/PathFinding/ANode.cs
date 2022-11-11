using System.Collections;
using UnityEngine;

[System.Serializable]
public class ANode
{
    public bool isWalkable;
    public Vector3 world_Pos;
    public int gridX;
    public int gridY;
    public float height;

    public int gCost;
    public int hCost;
    public ANode parentNode;
    public Tile myTile;


    public ANode(bool isWalkable, Vector3 world_Pos, int gridX, int gridY, Tile tile, float height = 0)
    {
        this.isWalkable = isWalkable;
        this.world_Pos = world_Pos;
        this.gridX = gridX;
        this.gridY = gridY;
        this.height = height;
        myTile = tile;
        if(tile == null)
        { 
            //this.isWalkable = false; 
        }
        else
        {
            myTile.Init(gridX, gridY,height, isWalkable, this);
        }
    }

    public int fCost => gCost + hCost;
}
