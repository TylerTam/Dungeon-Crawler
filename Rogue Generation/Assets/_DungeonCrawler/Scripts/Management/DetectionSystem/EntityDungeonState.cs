using UnityEngine;

public class EntityDungeonState : MonoBehaviour
{
    public bool m_inRoom;
    public DungeonGridCell m_currentCell;

    public Transform predictedPlace;

    public void Reinitialize()
    {
        m_currentCell = null;
        m_inRoom = false;

    }
    public void UpdateCellAttendance()
    {
        if (m_currentCell != null && m_inRoom)
        {
            
            if (!m_currentCell.IsWithinCell(predictedPlace.position))
            {
                if (m_currentCell.m_currentCellType == DungeonGridCell.CellType.Room)
                {
                    DungeonManager.Instance.RemoveEntityFromRoom(m_currentCell.m_roomIndex, gameObject);
                }
                DungeonGridCell currentGrid = DungeonManager.Instance.CurrentCell(predictedPlace.position);
                if (currentGrid.IsWithinCell(predictedPlace.position))
                {
                    m_inRoom = true;
                    m_currentCell = currentGrid;
                }
                else
                {
                    m_currentCell = null;
                    m_inRoom = false;
                }
            }
        }
        else
        {
            DungeonGridCell currentGrid = DungeonManager.Instance.CurrentCell(predictedPlace.position);
            if (currentGrid.IsWithinCell(predictedPlace.position))
            {
                m_inRoom = true;
                m_currentCell = currentGrid;
                if (m_currentCell.m_currentCellType == DungeonGridCell.CellType.Room)
                {
                    DungeonManager.Instance.AssignEntityToRoom(m_currentCell.m_roomIndex, gameObject);
                }
            }
            else
            {
                m_currentCell = null;
                m_inRoom = false;
            }
        }

    }
}
