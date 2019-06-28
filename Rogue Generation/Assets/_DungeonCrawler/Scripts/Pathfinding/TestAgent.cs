using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : MonoBehaviour
{
    DungeonNavigation_Agent agent;

    public Transform endPos;

    List<Node> path;
    private void Awake()
    {
        agent = GetComponent<DungeonNavigation_Agent>();
    }

    private void OnEnable()
    {
        path = agent.CreatePath(transform.position, endPos.position);
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;
        foreach(Node currentNode in path)
        {
            float size = agent.navGrid.m_nodeRadius;
            Gizmos.color = Color.black;
            Gizmos.DrawCube(currentNode.worldPosition, new Vector3(size*2,size * 2, size * 2));
        }
    }



}
