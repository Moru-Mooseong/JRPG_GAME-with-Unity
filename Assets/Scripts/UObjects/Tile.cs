using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class Tile : UObject
{
    #region Field
    public ANode node;

    public UObject above_Object;

    public bool isWalkable { get; private set; }
    [SerializeField] private int x;
    [SerializeField] private int z;
    [SerializeField] private float height;

    [SerializeField] private int hCost;
    [SerializeField] private int gCost;
    #endregion

    #region Properties
    public int X => x;
    public int Z => z;
    public float Height => height;
    #endregion

    #region Methods
    public void Init(int x, int z, float height, bool isWalkable, ANode node)
    {
        this.x = x;
        this.z = z;
        this.height = height;
        this.isWalkable = isWalkable;
        this.node = node;
    }

    public void SetAboveObject(UObject obj)
    {
        above_Object = obj;
        if (above_Object == null) isWalkable = true;
        else
        {
        }
    }

    public (int x, int z, float height) GetTIleNode()
    {
        return (x, z, height);
    }
    #endregion
}
