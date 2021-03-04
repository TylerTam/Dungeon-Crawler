using UnityEngine;

public class EntityDungeonState : MonoBehaviour
{
    public bool m_inRoom;
    public int m_roomIndex;
    public int m_cellType;

    public Transform predictedPlace;

    public void Reinitialize()
    {
        m_inRoom = false;

    }
    public void UpdateCellAttendance(Vector2 p_targetPos)
    {
        m_cellType = DungeonGenerationManager.Instance.GetWallCheck(p_targetPos.x, p_targetPos.y);
        if (m_cellType > GlobalVariables.m_startingWalkable)
        {
            m_roomIndex = m_cellType - GlobalVariables.m_startingWalkable - 1;
            m_inRoom = true;
        }
        else
        {
            m_inRoom = false;
        }

    }
}
