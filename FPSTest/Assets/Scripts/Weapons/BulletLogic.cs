using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour {
    public BulletType m_bulletType;

    public MeshRenderer m_meshRenderer;
    public GameObject m_explosionParticleGO;
    public GameObject m_trailParticleGO;
    public float m_expiryTime = 10;
    public float m_forwardImpulseForce = 500;
    public float m_AoeRadius;

	// Use this for initialization
	void Start () {
        StartCoroutine("Expire");
        AddForwardForce();
    }

    void AddForwardForce()
    {
        switch (m_bulletType)
        {
            case BulletType.Impulse:
                this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * m_forwardImpulseForce, ForceMode.Impulse);
                break;
            case BulletType.Repulsion:
                StartCoroutine(AddRepulsionForce());
                break;
        }
    }

    IEnumerator AddRepulsionForce()
    {
        float initialForce = m_forwardImpulseForce;
        this.transform.eulerAngles -= Vector3.right * 7;
        while (true)
        {
            initialForce --;
            this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * initialForce, ForceMode.Force);
            this.transform.eulerAngles += new Vector3(Random.Range(-1,2), Random.Range(-1, 2), Random.Range(-1, 2));
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (this.isActiveAndEnabled)
        {
            StartCoroutine(AnimateAndDestroy());
            StopCoroutine("Expire");
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, m_AoeRadius > 0 ? m_AoeRadius : 1);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag == "AI")
                {
                    hitColliders[i].GetComponent<AIController>().OnGotHit(PlayerController.Instance.PlayerWeaponSystem.CurrentWeapon.m_damage);
                }
            }
        }
    }

    IEnumerator AnimateAndDestroy()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        m_meshRenderer.enabled = false;
        m_explosionParticleGO.SetActive(true);
        if(m_trailParticleGO != null)
            m_trailParticleGO.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        Destroy(this.gameObject);
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
        StartCoroutine(AnimateAndDestroy());
    }
}

public enum BulletType { Impulse, Repulsion}
