using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DungeonNavigation : MonoBehaviour
{
    #region  variables
    [Header("Grid Variables")]
    public bool testGrid;

    public LayerMask m_terrain;
    public Vector2 m_gridWorldSize;
    public Vector3 m_gridOrigin;
    public float m_nodeRadius;
    Node[,] m_grid;
    float m_nodeDiameter;
    Vector2Int m_gridSize;
    #endregion


    #region Gizmos Settings
    [Header("Debug Settings")]
    public bool m_onlyDisplayPath;
    public bool m_displayGizmos;
    public Color m_unwalkableColor, m_walkableColor;
    #endregion
    private void Awake()
    {
        if (testGrid)
        {
            CreateGrid();
        }
    }

    public void CreateGrid()
    {
        m_gridSize = new Vector2Int();
        m_nodeDiameter = m_nodeRadius * 2;

        //So that no node is half off the space
        m_gridSize.x = Mathf.RoundToInt(m_gridWorldSize.x / m_nodeDiameter);
        m_gridSize.y = Mathf.RoundToInt(m_gridWorldSize.y / m_nodeDiameter);


        m_grid = new Node[m_gridSize.x, m_gridSize.y];

        //start at the bottom left of the m_grid
        Vector3 worldBottomLeft = m_gridOrigin - Vector3.right * m_gridWorldSize.x / 2 - Vector3.up * m_gridWorldSize.y / 2;
        for (int x = 0; x < m_gridSize.x; x++)
        {
            for (int y = 0; y < m_gridSize.y; y++)
            {

                //Get the world point of the current node, using the bottom left
                //Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + m_nodeRadius) + Vector3.forward * (y * nodeDiameter + m_nodeRadius);
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.up * (y * m_nodeDiameter + m_nodeRadius);
                bool isWalkable = !(Physics2D.OverlapCircle(worldPoint, .1f, m_terrain));//!(Physics2D.OverlapCircle(worldPoint, m_nodeRadius, m_terrain));
                m_grid[x, y] = new Node(isWalkable, worldPoint, x, y);

            }
        }

    }


    public int MaxSize
    {
        get
        {
            return m_gridSize.x * m_gridSize.y;
        }
    }


    private void OnDrawGizmos()
    {
        if (!m_displayGizmos) return;
        Gizmos.DrawWireCube(m_gridOrigin, new Vector3(m_gridWorldSize.x, m_gridWorldSize.y, 1));


        if (m_grid != null)
        {
            foreach (Node n in m_grid)
            {

                Gizmos.color = (n.m_walkable) ? m_walkableColor : m_unwalkableColor;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (m_nodeDiameter - .1f));
            }
        }

    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.m_gridPos.x + x;
                int checkY = node.m_gridPos.y + y;

                if (checkX >= 0 && checkX < m_gridSize.x && checkY >= 0 && checkY < m_gridSize.y)
                {
                    if (m_grid[checkX, checkY].m_walkable)
                    {
                        neighbours.Add(m_grid[checkX, checkY]);
                    }
                    /*
                    ///Why is this a raycast?
                    ///This was causing a memory overload, as it was being generated for a large path, for a few agents, hence overload.
                    ///Commeneted out, but not deelted, incase something arises later, and this may be able to help.
                    ///
                    if (!Physics2D.Raycast(node.worldPosition, m_grid[checkX, checkY].worldPosition - node.worldPosition, new Vector2(x, y).magnitude, m_terrain))
                    {
                        neighbours.Add(m_grid[checkX, checkY]);
                    }*/
                }
            }
        }
        return neighbours;
    }

    ///<Summary>
    //Called to gather a node using a world point
    public Node NodeFromWorldPoint(Vector3 p_worldPos)
    {


        float percentX = (p_worldPos.x - m_gridOrigin.x + m_gridWorldSize.x / 2) / m_gridWorldSize.x;
        float percentY = (p_worldPos.y - m_gridOrigin.y + m_gridWorldSize.y / 2) / m_gridWorldSize.y;

        //Create the percentage of the current position on the m_nodeGrid
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Calculate the actual positon, into an int
        int x = Mathf.RoundToInt((m_gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSize.y - 1) * percentY);
        return m_grid[x, y];
    }
    public Vector3 NodeToWorldPoint(Node navNode)
    {
        for (int x = 0; x < m_gridSize.x; x++)
        {
            for (int y = 0; y < m_gridSize.y; y++)
            {
                if (m_grid[x, y] == navNode)
                {
                    float xPos = (x * m_nodeDiameter) - m_gridWorldSize.x / 2;
                    float yPos = (y * m_nodeDiameter) - m_gridWorldSize.y / 2;
                    return new Vector3(xPos, 0f, yPos);
                }
            }
        }
        Debug.Log("Node does not exist in current m_grid. Defaulting to origin");
        return Vector3.zero;
    }


}
public class Node : IHeapItem<Node>
{
    public bool m_walkable;
    public Vector3 worldPosition;
    public Vector2Int m_gridPos;
    public int m_gCost, m_hCost;
    public Node m_parent;
    int heapIndex;

    public Node(bool m_walkable, Vector3 p_worldPos, int p_gridX, int p_gridY)
    {
        this.m_walkable = m_walkable;
        worldPosition = p_worldPos;

        m_gridPos = new Vector2Int(p_gridX, p_gridY);

    }

    public int fCost
    {
        get
        {
            return m_gCost + m_hCost;
        }
    }


    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node compareNode)
    {
        int compare = fCost.CompareTo(compareNode.fCost);

        if (compare == 0)
        {
            compare = m_hCost.CompareTo(compareNode.m_hCost);
        }

        return -compare;

    }
}

//Optimizes the nodes
public class Heap<T> where T : IHeapItem<T>
{

    T[] items;
    int currentItemCount;
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];

    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);

    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }


    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    SwapItem(item, items[swapIndex]);
                }
                else
                {
                    return;
                }

                //The parent has no children
            }
            else
            {
                return;
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                SwapItem(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void SwapItem(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int tempHeapIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tempHeapIndex;
    }
}


public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
