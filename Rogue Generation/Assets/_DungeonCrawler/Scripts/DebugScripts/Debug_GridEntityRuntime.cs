using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_GridEntityRuntime : MonoBehaviour
{
    public Vector2Int m_gridCell;

    private Vector2Int m_previousCell;


    private void Update()
    {
        if(m_gridCell != m_previousCell)
        {
            DungeonGenerationManager.Instance.AdjustEntityCheckGrid(m_previousCell.x, m_previousCell.y, null);
            DungeonGenerationManager.Instance.AdjustEntityCheckGrid(m_gridCell.x, m_gridCell.y, gameObject);
            m_previousCell = m_gridCell;
        }
    }
}
