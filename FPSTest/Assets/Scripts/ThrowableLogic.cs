using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableLogic : MonoBehaviour {

    public float m_expiryTimer = 3;
    public float m_explosionDuration = 2;
    public float m_explosionRadius = 1;
    public MeshRenderer m_meshRenderer;
    public ParticleSystem m_explosionParticle;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(RunExpiryTimer());
    }

    IEnumerator RunExpiryTimer()
    {
        float tempTimer = m_expiryTimer;
        while (tempTimer > 0)
        {
            tempTimer -= Time.deltaTime;
            yield return null;
        }
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, m_explosionRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
          if(hitColliders[i].tag == "AI")
            {
                hitColliders[i].GetComponent<AIController>().OnGotHit(PlayerController.Instance.PlayerWeaponSystem.CurrentThrowable.m_damage);
            }
        }

        m_meshRenderer.enabled = false;
        this.GetComponent<Rigidbody>().isKinematic = true;
        m_explosionParticle.gameObject.transform.position += Vector3.up * 0.3f;
        m_explosionParticle.gameObject.SetActive(true);

        yield return new WaitForSeconds(m_explosionDuration);
        Destroy(this.gameObject);
    }
}
