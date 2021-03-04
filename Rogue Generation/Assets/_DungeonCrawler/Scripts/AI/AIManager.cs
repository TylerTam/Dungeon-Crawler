using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    public int m_maxAiOnFloor = 5;
    public int m_currentAiOnFloor = 0;
    private List<GameObject> m_activeAiEntities = new List<GameObject>();
    private ObjectPooler m_pooler;


    [Header("Ai Varaibles")]
    public GameObject m_aiShellPrefab;
    public AIFloorData m_currentAIFloorData;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }


    public void AddAiEntity(GameObject newAi)
    {
        m_activeAiEntities.Add(newAi);
    }

    /// <summary>
    /// Called when a new floor is reached.
    /// Clears all enemies from the past floor, and returns them to the object pooler
    /// </summary>
    public void ClearFloor()
    {
        foreach (GameObject entity in m_activeAiEntities)
        {
            m_pooler.ReturnToPool(entity);
        }
        m_activeAiEntities.Clear();
        m_currentAiOnFloor = 0;
    }

    public void AIDefeated()
    {
        if (m_currentAiOnFloor > 0)
        {
            m_currentAiOnFloor--;
        }
    }

    public void UpdateAIFloorData(AIFloorData p_newData)
    {
        m_currentAIFloorData = p_newData;
    }

    public void AttemptAISpawn()
    {
        if (m_currentAiOnFloor < m_maxAiOnFloor)
        {
            float chance = Random.Range(0f, 1f);
            if (chance < m_currentAIFloorData.m_chanceOfAiSpawn)
            {
                Debug.Log("Spawn Ai");
                GameObject newAI = SpawnAi();
                AddAiEntity(newAI);
                newAI.GetComponent<EntityContainer>().m_turnBasedAgent.SetupCellAttendence();
                TurnBasedManager.Instance.NewAgent(newAI);
                m_currentAiOnFloor++;
            }
        }
    }

    public GameObject SpawnAi()
    {
        GameObject newAi = ObjectPooler.instance.NewObject(m_aiShellPrefab, Vector3.zero, Quaternion.identity);
        float aiDataChance = Random.Range(0, 1f);
        AIFloorData.AISpawnData currentData = new AIFloorData.AISpawnData();
        foreach (AIFloorData.AISpawnData data in m_currentAIFloorData.m_aiFloorData)
        {
            if (data.m_rarity > aiDataChance)
            {
                currentData = data;
                break;
            }
        }

        newAi.GetComponent<AIController>().InitializeAi(currentData.m_entityType, Random.Range(currentData.m_minLevel, currentData.m_maxLevel));
        List<Vector2> randomSpawnPos = new List<Vector2>();
        foreach (RoomData room in DungeonGenerationManager.Instance.m_floorData.m_allRooms)
        {
            foreach (Vector2Int pos in room.m_enemySpawnLocations)
            {
                if (DungeonGenerationManager.Instance.IsCellClear(room.m_roomCenterWorldPos.x + pos.x, room.m_roomCenterWorldPos.y + pos.y))
                {
                    randomSpawnPos.Add(new Vector2Int(room.m_roomCenterWorldPos.x + pos.x, -(room.m_roomCenterWorldPos.y + pos.y)));
                }
            }
        }
        Vector2 spawnPos = randomSpawnPos[Random.Range(0, randomSpawnPos.Count)];
        newAi.transform.position = spawnPos + new Vector2(0.5f, -0.5f);
        return newAi;
    }
}
