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
        Debug.Log("CheckAndDropItem");
        if(Random.Range(0,101) <= m_dropChance)
        {
            Debug.Log("Dropping");
            if (PlayerController.Instance.CurrentHealth <= m_healthDropThreshold)
            {
                Instantiate(m_healthDropablePrefab, new Vector3(pos.x, m_healthDropablePrefab.transform.position.y, pos.z), Quaternion.identity) ;
            }
            else if (PlayerController.Instance.gameObject.GetComponent<WeaponSystemLogic>().CurrentWeaponTotalAmmo < m_ammoDropThreshold)
            {
                Instantiate(m_ammoDropablePrefab, new Vector3(pos.x, m_ammoDropablePrefab.transform.position.y, pos.z), Quaternion.identity);
            }
        }
    }
}
