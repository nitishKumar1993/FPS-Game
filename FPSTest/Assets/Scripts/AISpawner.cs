using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour {
    private static AISpawner _instance;

    public GameObject m_aiNormalPrefab;
    public GameObject m_aiMediumPrefab;

    public float m_mediumAISpawnChance = 30;

    public float m_intervalRangeMin = 1.0f;
    public float m_intervalRangeMax = 3.0f;

    public List<Transform> m_spawnPointsList;

    public int m_maxAISpawnLimit = 20;

    public List<GameObject> m_spawnedAIsList = new List<GameObject>();

    public static AISpawner Instance
    {
        get { return _instance; }
    }

    // Use this for initialization
    void Start () {
        _instance = this;
        StartCoroutine(SpawnAIs());
	}
	
    IEnumerator SpawnAIs()
    {
        while (!PlayerController.Instance.IsPlayerDead)
        {
            if (m_spawnedAIsList.Count < m_maxAISpawnLimit)
            {
                GameObject tempPrefab = Random.Range(0, 101) > m_mediumAISpawnChance ? m_aiNormalPrefab : m_aiMediumPrefab;
                GameObject tempAI = Instantiate(tempPrefab, m_spawnPointsList[Random.Range(0, m_spawnPointsList.Count - 1)].position, Quaternion.identity) as GameObject;
                m_spawnedAIsList.Add(tempAI);
                yield return new WaitForSeconds(Random.Range(m_intervalRangeMin, m_intervalRangeMax));
            }
            else
                yield return null;
        }
    }

    public void DestroyAI(GameObject AI)
    {
        m_spawnedAIsList.Remove(AI);
        Destroy(AI);
    }
}
