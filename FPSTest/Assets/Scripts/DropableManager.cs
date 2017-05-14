using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropableManager : MonoBehaviour {
    private static DropableManager _instance;

    public GameObject m_ammoDropablePrefab;
    public GameObject m_healthDropablePrefab;

    public float m_dropChance = 60;
    public float m_ammoDropThreshold = 80;
    public float m_healthDropThreshold = 50;

    // Use this for initialization
    void Start () {
        _instance = this;
	}

    public static DropableManager Instance
    {
        get { return _instance; }
    }
	
    public void CheckAndDropItem(Vector3 pos)
    {
        if(Random.Range(0,101) <= m_dropChance)
        {
            GameObject tempDropable = null;
            if (PlayerController.Instance.CurrentHealth > m_healthDropThreshold/2)
            {
                if (PlayerController.Instance.PlayerWeaponSystem.CurrentWeaponTotalAmmo < m_ammoDropThreshold)
                {
                    tempDropable = Instantiate(m_ammoDropablePrefab, pos , Quaternion.identity) as GameObject;
                }
                else if (PlayerController.Instance.CurrentHealth <= m_healthDropThreshold)
                {
                    tempDropable = Instantiate(m_healthDropablePrefab, pos , Quaternion.identity) as GameObject;
                }
            }
            else 
            {
                tempDropable = Instantiate(m_healthDropablePrefab, pos , Quaternion.identity);
            }
            if (tempDropable != null)
            {
                Vector3 tempPos = tempDropable.transform.FindChild("Plane").transform.position;
                tempDropable.transform.FindChild("Plane").transform.position = new Vector3(tempPos.x, 0, tempPos.z);
            }
        }
    }
}
