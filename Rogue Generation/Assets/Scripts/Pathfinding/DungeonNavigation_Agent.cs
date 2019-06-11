﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNavigation_Agent : MonoBehaviour
{
    public DungeonNavigation navGrid;

    ///<Summary>
    ///Calculates the path towards the target
    public List<Node> CreatePath(Vector3 startPoint, Vector3 targetPoint)
    {
        //Gets both positions, in terms of nodes
        Node startNode = navGrid.NodeFromWorldPoint(startPoint);
        Node endNode = navGrid.NodeFromWorldPoint(targetPoint);

        Heap<Node> openNodes = new Heap<Node>(navGrid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();
        openNodes.Add(startNode);


        while (openNodes.Count > 0)
        {


            Node currentNode = openNodes.RemoveFirst();
            closedNodes.Add(currentNode);

            //If its the target node, stop calculating
            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }


            foreach (Node neighbour in navGrid.GetNeighbours(currentNode))
            {

                if (!neighbour.m_walkable || closedNodes.Contains(neighbour))
                {
                    continue;
                }


                int newMoveCostToNeighbour = currentNode.m_gCost + GetDistance(currentNode, neighbour);
                if (newMoveCostToNeighbour < neighbour.m_gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.m_gCost = newMoveCostToNeighbour;
                    neighbour.m_hCost = GetDistance(neighbour, endNode);
                    neighbour.m_parent = currentNode;

                    if (!openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }
        Debug.Log("Path could not be calculated!");
        return null;
    }

    ///<Summary>
    ///Returns the path to the target
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_parent;
        }
        path.Reverse();

        return path;
    }

    ///<Summary>
    ///Used when finding the costs for the nodes
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.m_gridPos.x - nodeB.m_gridPos.x);
        int distY = Mathf.Abs(nodeA.m_gridPos.y - nodeB.m_gridPos.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
