using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    public int m_damage = 10;
    public int m_pointsOnDeath = 10;

    // Use this for initialization
    void Start () {
        StartCoroutine(MoveTowardPlayer());
	}

    IEnumerator MoveTowardPlayer()
    {
        while(!PlayerController.Instance.IsPlayerDead)
        {
            this.GetComponent<NavMeshAgent>().SetDestination(PlayerController.Instance.transform.position);
            yield return new WaitForSeconds(1);
        }
        this.GetComponent<NavMeshAgent>().isStopped = true;
    }
	
	public void OnGotHit()
    {
        DropableManager.Instance.CheckAndDropItem(this.transform.position);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.tag == "Player")
            PlayerController.Instance.OnPlayerDamage(m_damage);
        ScoreManager.Instance.AddScore(m_pointsOnDeath);
    }
}
