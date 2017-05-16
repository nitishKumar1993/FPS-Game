using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystemLogic : MonoBehaviour {
    public List<int> m_currentWeaponsIndexList;
    public List<int> m_currentThrowablesIndexList;
    public int m_currentMeleeWeaponIndex;

    List<Weapon> m_weaponList = new List<Weapon>();
    List<Weapon> m_throwableList = new List<Weapon>();
    Weapon m_meleeWeapon;

    Weapon m_currentWeapon;
    int m_currentWeaponID = -1;

    Weapon m_currentThrowable;
    int m_currentThrowableID = -1;

    bool m_reloading = false;

    public List<Weapon> PlayerWeaponList
    {
        get { return m_weaponList; }
    }

    public void AddWeapon(int index,Transform parent)
    {
        if (WeaponsManager.Instance.m_weaponList.Count > index)
        {
            Debug.Log("Adding Weapon :" + WeaponsManager.Instance.m_weaponList[index].m_name);

            GameObject weaponHoster;
            if (parent.FindChild("Weapon") != null)
            {
                weaponHoster = parent.FindChild("Weapon").gameObject;
            }
            else
                weaponHoster = new GameObject();
            weaponHoster.name = "Weapon";
            weaponHoster.transform.SetParent(parent);
            weaponHoster.transform.localPosition = Vector3.zero;
            GameObject tempWeaponGO = Instantiate(WeaponsManager.Instance.m_weaponList[index].m_weaponGO, Vector3.zero, Quaternion.identity) as GameObject;
            tempWeaponGO.name = WeaponsManager.Instance.m_weaponList[index].m_name;
            tempWeaponGO.transform.SetParent(weaponHoster.transform);
            tempWeaponGO.transform.localPosition = Vector3.zero;

            WeaponsManager.Instance.m_weaponList[index].m_muzzleTransform = tempWeaponGO.transform.FindChild("Muzzle");
            WeaponsManager.Instance.m_weaponList[index].m_weaponGO = tempWeaponGO;

            Weapon tempWeapon = WeaponsManager.Instance.m_weaponList[index];
            tempWeapon.m_muzzleTransform = tempWeaponGO.transform.FindChild("Muzzle");
            tempWeapon.m_weaponGO = tempWeaponGO;

            m_weaponList.Add(tempWeapon);
        }
        else
            Debug.Log("Weapon ID doesnt exist");
    }

    public List<Weapon> PlayerThrowableList
    {
        get { return m_throwableList; }
    }

    public void AddThrowable(int index)
    {
        if (WeaponsManager.Instance.m_throwableList.Count > index)
        {
            Debug.Log("Adding Weapon :" + WeaponsManager.Instance.m_throwableList[index].m_name);
            m_throwableList.Add(WeaponsManager.Instance.m_throwableList[index]);
        }
        else
            Debug.Log("Throwable ID doesnt exist");
    }

    public void SetMeleeWeapon(int index, Transform parent)
    {
        if (WeaponsManager.Instance.m_meleeList.Count > index)
        {
            Debug.Log("Setting Melee Weapon :" + WeaponsManager.Instance.m_meleeList[index].m_name);
            m_meleeWeapon = WeaponsManager.Instance.m_meleeList[index];

            GameObject tempMeleeWeapon = Instantiate(WeaponsManager.Instance.m_meleeList[index].m_weaponGO, Vector3.zero, WeaponsManager.Instance.m_meleeList[index].m_weaponGO.transform.rotation) as GameObject;
            tempMeleeWeapon.name = WeaponsManager.Instance.m_meleeList[index].m_name;
            tempMeleeWeapon.transform.SetParent(parent.transform);
            tempMeleeWeapon.transform.localPosition = Vector3.zero;
            m_meleeWeapon.m_weaponGO = tempMeleeWeapon;
        }
        else
            Debug.Log("Melee ID doesnt exist");
       
    }

    public Weapon CurrentWeapon
    {
        get { return m_currentWeapon; }
    }

    public int CurrentWeaponID
    {
     get { return m_currentWeaponID; }
    }

    public Weapon CurrentThrowable
    {
        get { return m_currentThrowable; }
    }

    public int CurrentThrowableID
    {
        get { return m_currentThrowableID; }
    }

    //Ammo including in the active clip
    public int CurrentWeaponTotalAmmo
    {
        get { return m_currentWeapon.m_extraAmmo + m_currentWeapon.m_currentClipAmmo; }
    }

    public void Init(Transform parentTransform,int defaultWeaponIndex,int defaultThrowableIndex)
    {
        for (int i = 0; i < m_currentWeaponsIndexList.Count; i++)
        {
            AddWeapon(m_currentWeaponsIndexList[i], parentTransform);
        }
        for (int i = 0; i < m_currentThrowablesIndexList.Count; i++)
        {
            AddThrowable(m_currentThrowablesIndexList[i]);
        }
        SetMeleeWeapon(m_currentMeleeWeaponIndex, parentTransform);

        SwitchWeapon(defaultWeaponIndex);
        SwitchThrowable(defaultWeaponIndex);
    }

    public void SwitchWeapon(int WeaponId)
    {
        Debug.Log("SwitchWeapon :" + WeaponId);
        if (WeaponId < m_weaponList.Count && m_currentWeaponID != WeaponId)
        {
            StopReload();
            m_currentWeaponID = WeaponId;
            StopCoroutine("AnimateAndSwitchWeapon");
            StartCoroutine("AnimateAndSwitchWeapon");
        }
        else
        {
            Debug.Log("Weopon id not found in the list or already equipped");
        }
    }

    IEnumerator AnimateAndSwitchWeapon()
    {
        if (CurrentWeapon != null)
            CurrentWeapon.m_weaponGO.GetComponent<Animator>().SetBool("DrawIn", false);

        yield return new WaitForSeconds(0.2f);

        foreach (Weapon item in m_weaponList)
        {
            item.m_weaponGO.SetActive(false);
        }
        m_currentWeapon = m_weaponList[m_currentWeaponID];
        m_currentWeapon.m_weaponGO.SetActive(true);
        CurrentWeapon.m_weaponGO.GetComponent<Animator>().SetBool("DrawIn", true);
        if (m_currentWeapon.m_currentClipAmmo == 0)
            ReloadAmmo();
        GameManager.Instance.UpdateAmmoAmountHUD();

        GameManager.Instance.UpdateWeaponHUD();
    }

    public void SwitchThrowable(int id)
    {
        Debug.Log("SwitchThrowable :" + id);
        m_currentThrowableID = id;
        m_currentThrowable = m_throwableList[id];
        GameManager.Instance.UpdateThrowableAmountHUD();
        GameManager.Instance.UpdateThrowableHUD();
    }

    public void AddAmmo(int amount)
    {
        if ((m_currentWeapon.m_extraAmmo + m_currentWeapon.m_currentClipAmmo + amount) <= m_currentWeapon.m_maxAmmo)
            m_currentWeapon.m_extraAmmo += amount;
        else m_currentWeapon.m_extraAmmo += (m_currentWeapon.m_maxAmmo - (m_currentWeapon.m_extraAmmo + m_currentWeapon.m_currentClipAmmo));

        GameManager.Instance.UpdateAmmoAmountHUD();
    }

    public void ReloadAmmo()
    {
        if (m_currentWeapon.m_extraAmmo > 0 && m_currentWeapon.m_currentClipAmmo < m_currentWeapon.m_ammoPerClip && !m_reloading)
        {
            GameManager.Instance.ShowToast("Reloading...",true);
            StartCoroutine("ReloadAmmoCR");
        }
        else
        {
            if(m_currentWeapon.m_extraAmmo <= 0)
                GameManager.Instance.ShowToast("No Ammo");
            else if(m_currentWeapon.m_currentClipAmmo == m_currentWeapon.m_ammoPerClip)
                GameManager.Instance.ShowToast("Ammo full");
            else if (m_reloading)
                GameManager.Instance.ShowToast("Wait, reloading...",true);
        }
    }

    public void StopReload()
    {
        if (m_reloading)
        {
            GameManager.Instance.ShowToast("Can't Reload while Sprinting", true);
            Debug.Log("StopReload");
            m_currentWeapon.m_weaponGO.GetComponent<Animator>().SetTrigger("ReloadCancel");
            StopCoroutine("ReloadAmmoCR");
            m_reloading = false;
        }
    }

    IEnumerator ReloadAmmoCR()
    {
        m_reloading = true;
        bool success = false;
        float timer = m_currentWeapon.m_reloadTime;
        m_currentWeapon.m_weaponGO.GetComponent<Animator>().SetTrigger("Reload");
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0.1f)
            {
                success = true;
            }
            yield return null;
        }
        if (success)
        {
            int ammoToAdd = m_currentWeapon.m_extraAmmo >= m_currentWeapon.m_ammoPerClip ? (m_currentWeapon.m_ammoPerClip - m_currentWeapon.m_currentClipAmmo) : m_currentWeapon.m_extraAmmo;
            m_currentWeapon.m_currentClipAmmo += ammoToAdd;
            m_currentWeapon.m_extraAmmo -= ammoToAdd;
            GameManager.Instance.UpdateAmmoAmountHUD();
        }
        m_reloading = false;
    }

    public void Shoot()
    {
        if (!m_attackingMelee)
        {
            //Add recoil code here
            if (m_currentWeapon.m_currentClipAmmo > 0)
            {
                StopCoroutine("ShootCR");
                StartCoroutine("ShootCR");
            }
            else
                ReloadAmmo();
        }
    }


    IEnumerator ShootCR()
    {
        while (Input.GetMouseButton(0) && m_currentWeapon.m_currentClipAmmo > 0 && !m_reloading)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            RaycastHit hitInfo;
           
            GameObject tempBullet = Instantiate(m_currentWeapon.m_ammoPrefab, m_currentWeapon.m_muzzleTransform.position, Quaternion.identity) as GameObject;

            if (m_currentWeapon.m_fireParticleGO != null)
            {
                GameObject tempParticle;
                if (m_currentWeapon.m_muzzleTransform.childCount == 0)
                {
                    tempParticle = Instantiate(m_currentWeapon.m_fireParticleGO, m_currentWeapon.m_muzzleTransform.position, Quaternion.identity) as GameObject;
                    tempParticle.transform.SetParent(m_currentWeapon.m_muzzleTransform);
                }
                else
                {
                    tempParticle = m_currentWeapon.m_muzzleTransform.GetChild(0).gameObject;
                }
                tempParticle.transform.localEulerAngles = Vector3.zero;
                tempParticle.GetComponent<ParticleSystem>().Play();
            }

            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                tempBullet.transform.LookAt(hitInfo.point);
            }
            else
            {
                tempBullet.transform.LookAt(ray.GetPoint(500));
            }
            m_currentWeapon.m_currentClipAmmo--;
            GameManager.Instance.UpdateAmmoAmountHUD();
            yield return new WaitForSeconds(m_currentWeapon.m_shootInterval);
        }

        if (m_currentWeapon.m_currentClipAmmo <= 0)
        {
            ReloadAmmo();
        }
           
    }

    public void AttackMelee()
    {
        if(!m_attackingMelee)
        {
            StartCoroutine(AttackMeleeCR());
        }
    }

    bool m_attackingMelee = false;
    IEnumerator AttackMeleeCR()
    {
        m_attackingMelee = true;
        m_meleeWeapon.m_weaponGO.GetComponent<Animator>().SetTrigger("Stab");
        m_currentWeapon.m_weaponGO.transform.parent.gameObject.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 3))
        {
           if(hitInfo.transform.tag == "AI")
            {
                hitInfo.transform.GetComponent<AIController>().OnGotHit(PlayerController.Instance.PlayerWeaponSystem.m_meleeWeapon.m_damage);
            }
        }

        float tempTimer = m_meleeWeapon.m_reloadTime;
        while(tempTimer > 0)
        {
            tempTimer -= Time.deltaTime;
            yield return null;
        }
        m_currentWeapon.m_weaponGO.transform.parent.gameObject.SetActive(true);
        m_attackingMelee = false;
    }


    bool m_isLastThrowInProcess;
    public void UseThrowable()
    {
        if(m_currentThrowable.m_currentClipAmmo > 0 && !m_isLastThrowInProcess)
        {
            m_isLastThrowInProcess = true;
            GameObject tempThrowable = Instantiate(m_currentThrowable.m_weaponGO, this.GetComponent<PlayerController>().m_playerHeadGO.transform.position + Vector3.up * 0.15f, Quaternion.identity) as GameObject;
            tempThrowable.transform.eulerAngles = this.GetComponent<PlayerController>().m_playerHeadGO.transform.eulerAngles;
            float angleForce = -this.GetComponent<PlayerController>().m_playerHeadGO.transform.rotation.ToEulerAngles().x * 3.5f;
            tempThrowable.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * (10 + angleForce) + Vector3.up * 5, ForceMode.Impulse);

            // randomize the direction only in the case of hand grenade
            if (tempThrowable.GetComponent<ThrowableLogic>().m_type == ThrowableType.HandGrenade)
                tempThrowable.transform.localEulerAngles += Vector3.one * Random.Range(-60, 60);

            m_currentThrowable.m_currentClipAmmo--;
            GameManager.Instance.UpdateThrowableAmountHUD();
            Invoke("ResetThrowableBool", m_currentThrowable.m_shootInterval);
        }
    }

    void ResetThrowableBool()
    {
        m_isLastThrowInProcess = false;
    }
}

public enum WeaponType { HandHeld,Throughable,Melee}

public enum ThrowableType {HandGrenade,Mine }

[System.Serializable]
public class Weapon
{
    public string m_name;
    public WeaponType m_type;
    public GameObject m_weaponGO;
    public int m_damage;
    public int m_currentClipAmmo;
    public int m_extraAmmo;
    public int m_ammoPerClip;
    public int m_maxAmmo;
    public float m_shootInterval;
    public float m_reloadTime;
    public float m_recoilForce;
    public GameObject m_ammoPrefab;
    public Transform m_muzzleTransform;
    public Sprite m_weaponSprite;
    public GameObject m_fireParticleGO;
    public bool m_isShootable = true;
}
