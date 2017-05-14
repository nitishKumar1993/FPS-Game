using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour {

    public GameObject m_aiNormalPrefab;
    public GameObject m_aiMediumPrefab;

    public float m_mediumAISpawnChance = 30;

    public float m_intervalRangeMin = 1.0f;
    public float m_intervalRangeMax = 3.0f;

    public List<Transform> m_spawnPointsList;

    // Use this for initialization
    void Start () {
        StartCoroutine(SpawnAIs());
	}
	
    IEnumerator SpawnAIs()
    {
        while (!PlayerController.Instance.IsPlayerDead)
        {
            GameObject tempPrefab = Random.Range(0, 101) > m_mediumAISpawnChance ? m_aiNormalPrefab : m_aiMediumPrefab;
            Instantiate(tempPrefab, m_spawnPointsList[Random.Range(0, m_spawnPointsList.Count - 1)].position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(m_intervalRangeMin, m_intervalRangeMax));
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
