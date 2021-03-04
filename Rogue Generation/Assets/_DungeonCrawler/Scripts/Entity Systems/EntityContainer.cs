using UnityEngine;

public class EntityContainer : MonoBehaviour
{
    public EntityVisualManager m_entityVisualManager;
    public Entity_MovementController m_movementController;
    public TurnBasedAgent m_turnBasedAgent;
    public AttackController m_attackController;
    public EntityTeam m_entityTeam;
    public EntityDungeonState m_dungeonState;
    public Health m_entityHealth;

    public EntityRuntimeStats m_runtimeStats;

    private EntityData m_currentEntityData;
    public void Reinitialize(EntityData p_entityData, int p_entityLevel)
    {
        m_entityVisualManager.AssignEntityData(p_entityData);
        m_currentEntityData = p_entityData;
        m_dungeonState.Reinitialize();
        m_turnBasedAgent.Reinitialize();
        m_attackController.InitializeAttack(p_entityData, p_entityLevel);
        m_runtimeStats.AssignData(this, p_entityData, p_entityLevel);

        m_entityHealth.InitializeData(this, m_runtimeStats);
    }

    public void CarryOverToNextFloor()
    {
        m_entityVisualManager.AssignEntityData(m_currentEntityData);
        m_dungeonState.Reinitialize();
    }
}
