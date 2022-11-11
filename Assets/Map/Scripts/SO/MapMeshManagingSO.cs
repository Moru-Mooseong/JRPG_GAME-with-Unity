using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public enum Edge
{ 
    UpLeft,
    UpRight,
    DownRight,
    DownLeft,
    Default
}


[CreateAssetMenu(fileName ="MapMeshSO", menuName ="SO/MapMeshSO")]
public class MapMeshManagingSO : ScriptableObject
{
    [SerializeField] private List<MapMeshManager> MeshManagers = new List<MapMeshManager>();
    private Dictionary<string, MapMeshManager> dic_MapManager;
    [SerializeField] private GameObject water;

    [SerializeField] private List<string> HeightList = new List<string>();
    private Dictionary<string, int> dic_Height;

    public bool TryGetMapMesh(string key, out GameObject mesh, Edge edge = Edge.Default)
    {
        mesh = null;
        if(dic_MapManager == null || MeshManagers.Count != dic_MapManager.Count)
        {
            OnInit();
        }
        if(dic_MapManager.TryGetValue(key, out var value))
        {
            mesh = value.Meshs[(int)edge].gameObject;
            return true;
        }
        else
        {
            Debug.Log($"메시 컬러키 : {key}");
        }
        mesh = water;
        return false;
    }
    public bool TryGetHeightInfo(string key, out int height)
    {
        height = 0;
        if (dic_Height == null ||HeightList.Count != dic_Height.Count)
        {
            OnInit();
        }
        if (dic_Height.TryGetValue(key, out height))
        {
            return true;
        }
        else
        {
            Debug.Log($"높이 컬러키 : {key}");
            return false;
        }
    }
    
    public void OnInit()
    {
        dic_MapManager = new Dictionary<string, MapMeshManager>();
        if (MeshManagers == null) return;
        for (int i = 0; i < MeshManagers.Count; i++)
        {
            dic_MapManager.Add(MeshManagers[i].PixelColor, MeshManagers[i]);
        }
        dic_Height = new Dictionary<string, int>();
        for (int i = 0; i < HeightList.Count; i++)
        {
            dic_Height.Add(HeightList[i], i);
        }
    }


    [System.Serializable]
    public class MapMeshManager
    {
        [SerializeField] string pixelColor;
        [SerializeField] private MapMeshElement[] meshs = new MapMeshElement[(int)Edge.Default+1];

        public MapMeshElement[] Meshs => meshs;
        public string PixelColor => pixelColor;
    }

    [System.Serializable]
    public struct MapMeshElement
    {
        [SerializeField] private Edge edge;
        [SerializeField] private GameObject instance_Object;

        public Edge Edge => edge;
        public GameObject gameObject => instance_Object;
    }



    [SerializeField] private MapObjectManager objectManager = new MapObjectManager();
    public MapObjectManager ObjectManager => objectManager;
    [System.Serializable]
    public class MapObjectManager
    {
        [SerializeField] private List<MapObjectElement> objectsElements;
        [SerializeField] private Dictionary<string, MapObjectElement> dic_Objects;
        bool isInit;
        public MapObjectManager()
        {
            OnInit();
        }

        public bool TryGetObstacle(string colorKey, out GameObject value)
        {
            if(objectsElements.Count != dic_Objects.Count)
            {
                OnInit();
            }
            if (dic_Objects.TryGetValue(colorKey, out var obj))
            {
                int index = Random.Range(0, obj.Objects.Length);
                value = obj.Objects[index];
                return true;
            }
            else { value = null;
                Debug.Log($"컬러키 : {colorKey}");
                return false; }
        }

        private void OnInit()
        {
            dic_Objects = new Dictionary<string, MapObjectElement>();
            if (objectsElements == null) return;
            for (int i = 0; i < objectsElements.Count; i++)
            {
                dic_Objects.Add(objectsElements[i].PixelColor, objectsElements[i]);
            }
        }
            
    }

    [System.Serializable]
    public struct MapObjectElement
    {
        [SerializeField] private string ObjectName;
        [SerializeField] private string pixelColorHex;
        [SerializeField] private GameObject[] objects;

        public string PixelColor => pixelColorHex;
        public GameObject[] Objects => objects;
    }
}
