using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDungeonManager : MonoBehaviour
{
    public int m_currentTeamAmount;
    public EntityContainer m_playerObject;

    public GameObject m_aiShellPrefab;
    public List<GameObject> m_playerTeam;


    private void Start()
    {
        m_currentTeamAmount = PlayerData.Instance.m_currentPlayerTeam.Count;
        InitializePlayerObject();
    }

    private void InitializePlayerObject()
    {
        m_playerObject.m_entityVisualManager.AssignEntityData(PlayerData.Instance.m_playerEntityData);
    }
    public void CreatePlayerTeam()
    {
        for (int i = 0; i < m_currentTeamAmount; i++)
        {

        }
    }
}
