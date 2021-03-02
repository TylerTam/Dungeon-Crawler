using System.Collections.Generic;
using UnityEngine;

public class DungeonNavigation_Agent : MonoBehaviour
{
    [HideInInspector]
    public DungeonNavigation navGrid;

    public bool m_drawDebugTools;
    private List<Node> m_debugPath;
    public Color m_pathColor;
    private void Start()
    {
        navGrid = DungeonNavigation.Instance;

    }

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
                m_debugPath = RetracePath(startNode, endNode);
                //return RetracePath(startNode, endNode);
                return m_debugPath;
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

    private void OnDrawGizmos()
    {
        if (!m_drawDebugTools) return;
        if (m_debugPath == null) return;
        if (m_debugPath.Count > 0)
        {
            Gizmos.color = m_pathColor;
            foreach (Node path in m_debugPath)
            {
                Gizmos.DrawCube(path.worldPosition, new Vector3(.25f, .25f));
            }
        }
    }
}
