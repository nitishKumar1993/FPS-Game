using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour {

    float m_expiryTime = 10;

	// Use this for initialization
	void Start () {
        StartCoroutine("Expire");
        this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 500, ForceMode.Impulse);
	}


    void OnCollisionEnter(Collision coll)
    {
        // Destroy(this.gameObject);
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<MeshRenderer>().enabled = false;
        this.transform.GetChild(0).gameObject.SetActive(true);
        Invoke("DestroyGO", 1);
        StopCoroutine("Expire");
        Debug.Log(coll.transform.tag);
        if (coll.transform.tag == "AI")
        {
            Debug.Log("OnAIHit");
            coll.transform.parent.GetComponent<AIController>().OnGotHit();
        }
    }

    void DestroyGO()
    {
        Destroy(this.gameObject);
    }

    IEnumerator Expire()
    {
        float tempTimer = m_expiryTime;
        while(tempTimer > 0)
        {
            tempTimer -= Time.deltaTime;
            yield return null;
        }
        DestroyGO();
    }
}
