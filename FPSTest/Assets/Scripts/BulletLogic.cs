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
        Invoke("DestroyGO", 0.5f);
        StopCoroutine("Expire");
        if (coll.transform.tag == "AI")
        {
            coll.transform.GetComponent<AIController>().OnGotHit(PlayerController.Instance.m_weaponSystem.CurrentWeapon.m_damage);
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
