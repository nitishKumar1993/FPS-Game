using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystemLogic : MonoBehaviour {

    public List<Weapon> m_weaponList;
    public List<Weapon> m_throughableList;
    public Weapon m_meleeWeapon;

    Weapon m_currentWeapon;
    int m_currentWeaponID = -1;

    bool m_reloading = false;

    public Weapon CurrentWeapon
    {
        get { return m_currentWeapon; }
    }

    public int CurrentWeaponID
    {
     get { return m_currentWeaponID; }
    }

    //Ammo including in the actove clip
    public int CurrentWeaponTotalAmmo
    {
        get { return m_currentWeapon.m_extraAmmo + m_currentWeapon.m_currentClipAmmo; }
    }

    public void SwitchWeapon(int WeaponId)
    {
        if (WeaponId < m_weaponList.Count && m_currentWeaponID != WeaponId)
        {
            m_currentWeaponID = WeaponId;
            foreach (Weapon item in m_weaponList)
            {
                item.m_weaponGO.SetActive(false);
            }
            m_currentWeapon = m_weaponList[WeaponId];
            m_currentWeapon.m_weaponGO.SetActive(true);
            if (m_currentWeapon.m_currentClipAmmo == 0)
                ReloadAmmo();
            this.GetComponent<PlayerController>().UpdateAmmoAmountHUD();
        }
        else
        {
            Debug.Log("Weopon id not found in the list or already equipped");
        }
    }

    public void AddAmmo(int amount)
    {
        if ((m_currentWeapon.m_extraAmmo + m_currentWeapon.m_currentClipAmmo + amount) <= m_currentWeapon.m_maxAmmo)
            m_currentWeapon.m_extraAmmo += amount;
        else m_currentWeapon.m_extraAmmo += (m_currentWeapon.m_maxAmmo - (m_currentWeapon.m_extraAmmo + m_currentWeapon.m_currentClipAmmo));

        this.GetComponent<PlayerController>().UpdateAmmoAmountHUD();
    }

    public void ReloadAmmo()
    {
        if (m_currentWeapon.m_extraAmmo > 0 && m_currentWeapon.m_currentClipAmmo < m_currentWeapon.m_ammoPerClip && !m_reloading)
        {
            GameManager.Instance.ShowToast("Reloading",true);
            StartCoroutine("ReloadAmmoCR");
        }
        else
        {
            if(m_currentWeapon.m_extraAmmo <= 0)
                GameManager.Instance.ShowToast("No Ammo");
            else if(m_currentWeapon.m_currentClipAmmo == m_currentWeapon.m_ammoPerClip)
                GameManager.Instance.ShowToast("Ammo full");
            else if (m_reloading)
                GameManager.Instance.ShowToast("Wait, reloading");
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
            this.GetComponent<PlayerController>().UpdateAmmoAmountHUD();
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
                StopCoroutine("Attack");
                StartCoroutine("Attack");
            }
            else
                GameManager.Instance.ShowToast("No more Ammo");
        }
    }

    IEnumerator Attack()
    {
        while (Input.GetMouseButton(0) && m_currentWeapon.m_currentClipAmmo > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            RaycastHit hitInfo;
           
            GameObject tempBullet = Instantiate(m_currentWeapon.m_ammoPrefab, m_currentWeapon.m_muzzleTransform.position, Quaternion.identity) as GameObject;
            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                tempBullet.transform.LookAt(hitInfo.point);
            }
            else
            {
                tempBullet.transform.LookAt(ray.GetPoint(500));
            }
            m_currentWeapon.m_currentClipAmmo--;
            this.GetComponent<PlayerController>().UpdateAmmoAmountHUD();
            yield return new WaitForSeconds(m_currentWeapon.m_shootInterval);
        }
        if (m_currentWeapon.m_currentClipAmmo <= 0)
            GameManager.Instance.ShowToast("No Ammo");
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
                hitInfo.transform.GetComponent<AIController>().OnGotHit(PlayerController.Instance.m_weaponSystem.m_meleeWeapon.m_damage);
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
}

public enum WeaponType { HandHeld,Throughable,Melee}

[System.Serializable]
public class Weapon
{
    public WeaponType m_type;
    public GameObject m_weaponGO;
    public int m_damage;
    public int m_currentClipAmmo;
    public int m_extraAmmo;
    public int m_ammoPerClip;
    public int m_maxAmmo;
    public float m_shootInterval;
    public float m_reloadTime;
    public float m_recoilforce;
    public GameObject m_ammoPrefab;
    public Transform m_muzzleTransform;
    public Sprite m_weaponSprite;
    public bool m_isShootable = true;
}
