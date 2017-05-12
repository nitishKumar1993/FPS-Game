using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystemLogic : MonoBehaviour {

    public List<Weapon> m_weaponList;
    public List<Weapon> m_throughableList;

    Weapon m_currentWeapon;
    int m_currentWeaponID = -1;

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
        get { return m_currentWeapon.m_totalAmmo + m_currentWeapon.m_currentClipAmmo; }
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
        m_currentWeapon.m_totalAmmo += amount;
        this.GetComponent<PlayerController>().UpdateAmmoAmountHUD();
    }

    public void ReloadAmmo()
    {
        if (m_currentWeapon.m_totalAmmo > 0)
        {
            int ammoToAdd = (m_currentWeapon.m_ammoPerClip - m_currentWeapon.m_currentClipAmmo);
            m_currentWeapon.m_currentClipAmmo += ammoToAdd;
            m_currentWeapon.m_totalAmmo -= ammoToAdd;
            this.GetComponent<PlayerController>().UpdateAmmoAmountHUD();
        }
        else
        {
            Debug.Log("No Ammo");
        }
    }

    public void Shoot()
    {
        StopCoroutine("Attack");
        StartCoroutine("Attack");
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
    }
}

public enum WeaponType { HandHeld,Throughable}

[System.Serializable]
public class Weapon
{
    public WeaponType m_type;
    public GameObject m_weaponGO;
    public int m_totalAmmo;
    public int m_maxAmmo;
    public int m_ammoPerClip;
    public int m_currentClipAmmo;
    public float m_shootInterval;
    public GameObject m_ammoPrefab;
    public Transform m_muzzleTransform;
    public Sprite m_weaponSprite;
    public bool m_isShootable = true;

    public int Clips()
    {
        return (int)m_totalAmmo / m_ammoPerClip;
    }
}
