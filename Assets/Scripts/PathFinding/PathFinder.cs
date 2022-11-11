using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A* 알고리즘을 활용한 패스파인딩
/// </summary>
public class PathFinder : MonoBehaviour
{
    //Agrid grid;
    AutoMapGenerator mapManager;


    public Tile start;
    public Tile end;

    private void Awake()
    {
        //grid = GetComponent<Agrid>();
        mapManager = FindObjectOfType<AutoMapGenerator>();
    }
    private void Start()
    {
    }
    private void Update()
    {
        if(start != null && end != null)
        FindPath(start, end);
    }

    private void FindPath(Tile start, Tile end)
    {
        ANode startNode =
            start.node;
            //grid.GetNodeFromWorldPoint(start);
        ANode endNode =
            end.node;
            //grid.GetNodeFromWorldPoint(end);

        List<ANode> openList = new List<ANode>();
        HashSet<ANode> closedList = new HashSet<ANode>();
        openList.Add(startNode);
        while(openList.Count > 0)
        {
            ANode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost ||
                    openList[i].fCost == currentNode.fCost && 
                    openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //탐색 종료
            if(currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            //계속 탐색
            foreach(ANode n in mapManager.GetNeighbour(currentNode))
            {
                if(!n.isWalkable || closedList.Contains(n) || Mathf.Abs(n.height - currentNode.height )>= 1)
                {
                    continue;
                }

                int newCurrentToNeighbourCost = currentNode.gCost + GetDistanceCost(currentNode, n);
                if(newCurrentToNeighbourCost < n.gCost || !openList.Contains(n))
                {
                    n.gCost = newCurrentToNeighbourCost;
                    n.hCost = GetDistanceCost(n, endNode);
                    n.parentNode = currentNode;

                    if(!openList.Contains(n))
                    {
                        openList.Add(n);
                    }

                }
            }
        }
        
    }

    private int GetDistanceCost(ANode NodeA, ANode NodeB)
    {
        int distX = Mathf.Abs(NodeA.gridX - NodeB.gridX);
        int disty = Mathf.Abs(NodeA.gridY - NodeB.gridY);
        
        if(distX > disty)
        {
            return 14 * disty + 10 * (distX - disty);
        }
        else
        {
            return 14 * disty + 10 * (disty - distX);
        }
    }

    private void RetracePath(ANode startNode, ANode endNode)
    {
        List<ANode> path = new List<ANode>();
        ANode currentNode = endNode;
        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();
        mapManager.path = path;
    }

}
