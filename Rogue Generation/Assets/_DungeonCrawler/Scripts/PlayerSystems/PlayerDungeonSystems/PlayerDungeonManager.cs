using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDungeonManager : MonoBehaviour
{
    public static PlayerDungeonManager Instance;
    public int m_currentTeamAmount;
    public EntityContainer m_playerEntityContainer;

    public GameObject m_aiShellPrefab;
    public List<GameObject> m_playerTeam;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_currentTeamAmount = PlayerData.Instance.m_currentPlayerTeam.Count;
        InitializePlayerObject();
    }

    private void InitializePlayerObject()
    {
        m_playerEntityContainer.Reinitialize(PlayerData.Instance.m_playerEntityData);
        
    }

    public void ResetPlayerComponents()
    {
        m_playerEntityContainer.CarryOverToNextFloor();
    }
    public void CreatePlayerTeam()
    {
        for (int i = 0; i < m_currentTeamAmount; i++)
        {

        }
    }
}
