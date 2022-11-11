using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AutoMapGenerator : MonoBehaviour
{


    public Texture2D mapDesign_Texture;
    public Texture2D mapDesign_Texture2;
    public Texture2D material_Texture;
    public Texture2D Objects_Texture;
    public MapMeshManagingSO mapMeshSO;
    public bool isInit;
    public GameObject CreateTest;

    public ANode[,] nodes;
    public int gridX;
    public int gridY;
    [ContextMenu("Generate")]
    public void Generate()
    {
        isInit = true;
        Init();
        TextureRead();
    }

    public List<ANode> GetNeighbour(ANode from)
    {
        List<ANode> neighbours = new List<ANode>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == y || Mathf.Abs(x) + Mathf.Abs(y) != 1) continue;
                int checkX = from.gridX + x;
                int checkY = from.gridY + y;

                if (checkX >= 0 && checkX < gridX && checkY >= 0 && checkY < gridY)
                {
                    neighbours.Add(nodes[checkX, checkY]);
                }
            }
        }
        Debug.Log($"neighbours count : {neighbours.Count}");
        return neighbours;
    }

    private void Start()
    {
        if (!isInit)
        {
            Generate();
        }
    }

    private void Init()
    {

    }

    private void TextureRead()
    {
        var textureSize = material_Texture.Size();
        gridX = (int)textureSize.x;
        gridY = (int)textureSize.y;
        var waters_Parent = new GameObject("Water");
        waters_Parent.transform.SetParent(this.transform);

        GameObject tiles_Parents = new GameObject("Tiles");
        tiles_Parents.transform.SetParent(this.transform);

        GameObject obstacle_Parent = new GameObject("Obstacle");
        obstacle_Parent.transform.SetParent(this.transform);

        nodes = new ANode[gridX, gridY];

        for (int x = 0; x < textureSize.x; x++)
        {
            for (int y = 0; y < textureSize.y; y++)
            {
                //노드 2차원배열에 담을 데이터그릇
                bool isWalkable = false;
                Tile nodeTile = null;
                int nodeHeight = 0;

                //물은 항상 생성
                mapMeshSO.TryGetMapMesh("water", out var waterOrigin);
                var obj = GameObject.Instantiate(waterOrigin, waters_Parent.transform);
                obj.transform.position = new Vector3(x, 0, y);

                //높이데이터 받아오기
                int height = 0;
                var heightHex = mapDesign_Texture2.GetPixel(x * 32, y * 32);
                mapMeshSO.TryGetHeightInfo(heightHex.ToHexString(), out height);

                //오브젝트 데이터 받아오기
                var objectHext = Objects_Texture.GetPixel(x * 32, y * 32);
                GameObject obstacle_Prefap = null;
                mapMeshSO.ObjectManager.TryGetObstacle(objectHext.ToHexString(), out obstacle_Prefap);

                //지형지물 생성
                for (int i = 0; i < height; i++)
                {
                    var colorHex = material_Texture.GetPixel(x, y);
                    if (mapMeshSO.TryGetMapMesh(colorHex.ToHexString(), out GameObject mesh))
                    {
                        mesh = ImportMeshObject(mesh, colorHex, x, y, i);
                        mesh.transform.SetParent(tiles_Parents.transform);

                        if (i == height - 1)
                        {
                            if (!mesh.TryGetComponent<Tile>(out nodeTile))
                            {
                                nodeTile = mesh.AddComponent<Tile>();
                            }
                            isWalkable = true;
                            nodeHeight = i;
                            if (obstacle_Prefap != null)
                            {
                                var obstacle =
                                    GameObject.Instantiate(obstacle_Prefap);
                                obstacle.transform.position =
                                    new Vector3(x, i + 1, y);
                                obstacle.transform.
                                    SetParent(obstacle_Parent.transform);
                                isWalkable = false;
                            }
                            mesh.gameObject.name = "Tile";

                        }
                    }
                }
                nodes[x, y] = new ANode(isWalkable, new Vector3(x, nodeHeight, y), x, y, nodeTile, nodeHeight);
            }
        }
    }

    /// <summary>
    /// 코드 수정
    /// </summary>
    /// <param name="pixelColor"></param>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private GameObject ImportMeshObject(GameObject originMesh, Color pixelColor, int _x, int _y, int height)
    {
        bool[,] isObjectExist = new bool[3, 3];
        List<bool> debug = new List<bool>();
        int checkX = _x;
        int checkY = _y;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == y || (x == 1 && y == -1) || (x == -1 && y == 1)) { debug.Add(true); isObjectExist[x + 1, y + 1] = true; continue; }
                if (checkX + x >= 0 && checkX + x < material_Texture.Size().x &&
                    checkY + y >= 0 && checkY + y < material_Texture.Size().x)
                {
                    var colorAround = material_Texture.GetPixel(checkX, checkY);
                    var heightAround = mapDesign_Texture2.GetPixel(checkY * 32, checkY * 32);
                    mapMeshSO.TryGetHeightInfo(heightAround.ToHexString(), out var heightInt);
                    if (//pixelColor == colorAround &&
                        heightInt <= height)
                    {
                        isObjectExist[x + 1, y + 1] = true;
                        debug.Add(true);
                    }
                    else
                    {
                        isObjectExist[x + 1, y + 1] = false;
                        debug.Add(false);
                    }
                }
                else
                {
                    isObjectExist[x + 1, y + 1] = false;
                    debug.Add(false);
                }
            }
        }
        Edge edge;
        const bool t = true;
        const bool f = false;
        bool tf = t || f;
        bool[,] UL = new bool[3, 3] { { tf, t, tf }, { f, t, t }, { tf, f, tf } };
        bool[,] UR = new bool[3, 3] { { tf, t, tf }, { t, t, f }, { tf, f, tf } };
        bool[,] DR = new bool[3, 3] { { tf, f, tf }, { t, t, f }, { tf, t, tf } };
        bool[,] DL = new bool[3, 3] { { tf, f, tf }, { f, t, t }, { tf, t, tf } };
        string debugValue = "";
        for (int i = 0; i < debug.Count; i++)
        {
            if (i % 3 == 0) debugValue += "\n";
            debugValue += debug[i];
            debugValue += "  ";
        }
        if (isObjectExist == UL) { edge = Edge.UpLeft; }


        else if (isObjectExist == UR) { edge = Edge.UpRight; }

        else if (isObjectExist == DR) { edge = Edge.DownRight; }


        else if (isObjectExist == DL) { edge = Edge.DownLeft; }

        else { edge = Edge.Default; }
        GameObject meshObj = null;
        if (originMesh != null)
        {
            meshObj = GameObject.Instantiate(originMesh);
            meshObj.transform.position = new Vector3(_x, height, _y);
        }
        Debug.Log($"bool 밸류 : {debugValue} \n 엣지 : {edge} \n 높이 : {height} oror {tf} \n count :");
        return meshObj;
    }

    public List<ANode> path;
    private void OnDrawGizmos()
    {
        Color[] colors = new Color[7]
        {
            Color.red,
            Color.Lerp(Color.red,Color.yellow,0.5f),
            Color.yellow,
            Color.green,
            Color.blue,
            Color.cyan,
            Color.Lerp(Color.red,Color.blue,0.5f)
        };
        if (path != null)
        {
            int count = 0;
            foreach (var n in path)
            {
                Gizmos.color = colors[count % 7];
                Vector3 look = new Vector3(0, 1, 0);
                Gizmos.DrawSphere(n.world_Pos + look, 0.3f);
                count++;
            }
        }
    }
}
