using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    public int m_damage = 10;
    public int m_pointsOnDeath = 10;

    public int m_totalhealth = 100;
    public int m_currentHealth = 100;
    public GameObject m_weaponHolderGO;

    WeaponSystemLogic m_weaponSystem;

    // Use this for initialization
    void Start () {
        m_weaponSystem = this.GetComponent<WeaponSystemLogic>();
       /* if(m_weaponSystem != null)
            m_weaponSystem.Init(m_weaponHolderGO.transform, 0, 0);*/

        m_currentHealth = m_totalhealth;
        StartCoroutine(MoveTowardPlayer());
    }

    IEnumerator MoveTowardPlayer()
    {
        while (!PlayerController.Instance.IsPlayerDead && this.GetComponent<NavMeshAgent>().enabled)
        {
            if (Vector3.Distance(PlayerController.Instance.transform.position, this.transform.position) > 2)
                this.GetComponent<NavMeshAgent>().SetDestination(PlayerController.Instance.transform.position);
            yield return new WaitForSeconds(1);
        }
        if (this.GetComponent<NavMeshAgent>().enabled)
            this.GetComponent<NavMeshAgent>().isStopped = true;
    }
	
	public void OnGotHit(int damage)
    {
        m_currentHealth -= damage;
        if (m_currentHealth <= 0)
        {
            this.GetComponent<NavMeshAgent>().enabled = false;
            this.GetComponent<CapsuleCollider>().enabled = false;
            Invoke("DestroyAndCheckDropables", 0.1f);
        }
        this.GetComponent<MeshRenderer>().material.color = new Color(1, 1 - ((float)(m_totalhealth - m_currentHealth)/ (float)m_totalhealth), 0, 1);
    }

    void DestroyAndCheckDropables()
    {
        ScoreManager.Instance.AddScore(m_pointsOnDeath);
        DropableManager.Instance.CheckAndDropItem(this.transform.position);
        AISpawner.Instance.DestroyAI(this.gameObject);
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.tag == "Player")
            PlayerController.Instance.OnPlayerDamage(m_damage);
    }
}
