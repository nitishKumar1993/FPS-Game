using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropableLogic : MonoBehaviour {
    public DropabeType m_type;

    public int m_healthHealValue = 10;
    public int m_ammoAmount= 20;
    public float m_expiryTimer = 5;

    // Use this for initialization
    void Start () {
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
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player")
        {
            StartCoroutine(ApplyEffect());
        }
    }

    IEnumerator ApplyEffect()
    {
        switch(m_type)
        {
            case DropabeType.Health:
                PlayerController.Instance.HealPlayer(m_healthHealValue);
                break;
            case DropabeType.Ammo:
                PlayerController.Instance.PlayerWeaponSystem.AddAmmo(m_ammoAmount);
                break;
        }

        float tempTimer = 1;
        this.GetComponent<BoxCollider>().enabled = false;
        Transform meshGO = this.transform.FindChild("Mesh");
        meshGO.GetComponent<MeshRenderer>().enabled = false;
        meshGO.FindChild("Particle System").gameObject.SetActive(false);
        meshGO.FindChild("Particle System2").gameObject.SetActive(true);
        while (tempTimer > 0)
        {

            yield return null;
        }
        Destroy(this.gameObject);
    }
}

public enum DropabeType { Health,Ammo}
