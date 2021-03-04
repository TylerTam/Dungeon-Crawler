using UnityEngine;

public class FloorObject_Staircase : MonoBehaviour, IFloorObject
{
    public static FloorObject_Staircase Instance;
    public LayerMask m_playerLayer;
    private void Awake()
    {
        Instance = this;
    }
    public void Interact(EntityContainer p_interactingEntity)
    {

        if (PlayerDungeonManager.Instance.m_playerEntityContainer == p_interactingEntity)
        {
            Input_Base.Instance.m_canPerform = false;
            DungeonGenerationManager.Instance.NewFloor();
        }

    }
}
