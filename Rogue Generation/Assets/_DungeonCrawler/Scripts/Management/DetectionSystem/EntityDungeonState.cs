using UnityEngine;

public class EntityDungeonState : MonoBehaviour
{
    public bool m_inRoom;
    public DungeonGridCell m_currentCell;

    public Transform predictedPlace;

    public void UpdateCellAttendance()
    {
        if (m_currentCell != null)
        {
            if (!m_currentCell.IsWithinCell(predictedPlace.position))
            {
                if (m_currentCell.m_currentCellType == DungeonGridCell.CellType.Room)
                {
                    m_currentCell.RemoveEntityFromRoom(gameObject);
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
                    m_currentCell.AddEntityToRoom(gameObject);
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
