using UnityEngine;

public class EntityDungeonState : MonoBehaviour
{
    public bool m_inRoom;
    //public DungeonGridCell m_currentCell;

    public Transform predictedPlace;

    public void Reinitialize()
    {
        m_inRoom = false;

    }
    public void UpdateCellAttendance()
    {
        

    }
}
