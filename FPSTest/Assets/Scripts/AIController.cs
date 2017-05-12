using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(MoveTowardPlayer());
	}

    IEnumerator MoveTowardPlayer()
    {
        while(true)
        {
            this.GetComponent<NavMeshAgent>().SetDestination(PlayerController.Instance.transform.position);
            yield return new WaitForSeconds(1);
        }
    }
	
	public void OnGotHit()
    {
        Destroy(this.gameObject);
    }
}
