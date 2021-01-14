using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    public int m_maxAiOnScene = 5;
    public int m_currentAiOnScene = 0;
    private List<GameObject> m_activeAiEntities = new List<GameObject>();
    private ObjectPooler m_pooler;
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
        foreach(GameObject entity in m_activeAiEntities)
        {
            m_pooler.ReturnToPool(entity);
        }
        m_activeAiEntities.Clear();
        m_currentAiOnScene = 0;
    }

    public void AIDefeated()
    {
        if(m_currentAiOnScene > 0)
        {
            m_currentAiOnScene--;
        }
    }
}
