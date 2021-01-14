using System.Collections;
using System.Collections.Generic;
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
}
