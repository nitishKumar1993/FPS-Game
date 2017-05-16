using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public GameObject m_toastGO;
    public GameObject m_quitPanelGO;

    public GameObject m_WeaponHUDThrowableTextGO;
    public GameObject m_WeaponHUDThrowableImgGO;
    public GameObject m_WeaponHUDThrowablesContainerGO;

    public GameObject m_HUDHealthTextGO;

    public GameObject m_WeaponHUDImgGO;
    public GameObject m_WeaponHUDAmmoTextGO;
    public GameObject m_WeaponHUDWeaponsContainerGO;
    public Image m_WeaponHUDBloodScreenImg;

    public GameObject m_gameoverGO;

    WeaponSystemLogic m_playerWeaponSystem;
    string m_toastMsg = "";

    public static GameManager Instance
    {
        get { return _instance; }
    }

	// Use this for initialization
	void Awake () {
        _instance = this;
        m_playerWeaponSystem = PlayerController.Instance.gameObject.GetComponent<WeaponSystemLogic>();
	}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!m_quitPanelGO.activeSelf)
            {
                m_quitPanelGO.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
                OnQuitCancelBtnClicked();
        }
    }

    public void HUDHealth(int currentHealth, int totalHealth)
    {
        m_HUDHealthTextGO.GetComponent<Text>().text = string.Format("{0}/{1}", currentHealth.ToString(), totalHealth.ToString());
    }

    public void UpdateAmmoAmountHUD()
    {
        m_WeaponHUDAmmoTextGO.GetComponent<Text>().text = string.Format("{0}/{1}", m_playerWeaponSystem.CurrentWeapon.m_currentClipAmmo, m_playerWeaponSystem.CurrentWeapon.m_extraAmmo);
    }

    public void UpdateWeaponHUD()
    {
        m_WeaponHUDImgGO.GetComponent<Image>().sprite = m_playerWeaponSystem.CurrentWeapon.m_weaponSprite;
        for (int i = 1; i <= m_playerWeaponSystem.PlayerWeaponList.Count; i++)
        {
            string tempName = "Weapon" + i.ToString();
            Transform currentWeaponGO = m_WeaponHUDWeaponsContainerGO.transform.FindChild(tempName);
            if (currentWeaponGO == null)
            {
                currentWeaponGO = (Instantiate(m_WeaponHUDWeaponsContainerGO.transform.FindChild("Weapon1").gameObject, Vector3.zero, Quaternion.identity) as GameObject).transform;
                currentWeaponGO.SetParent(m_WeaponHUDWeaponsContainerGO.transform, false);
                currentWeaponGO.name = tempName;
            }

            currentWeaponGO.FindChild("Image").GetComponent<Image>().sprite = m_playerWeaponSystem.PlayerWeaponList[i - 1].m_weaponSprite;
            if (m_playerWeaponSystem.PlayerWeaponList[i - 1] == m_playerWeaponSystem.CurrentWeapon)
            {
                currentWeaponGO.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            }
            else
                currentWeaponGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void UpdateThrowableAmountHUD()
    {
        m_WeaponHUDThrowableTextGO.GetComponent<Text>().text = m_playerWeaponSystem.CurrentThrowable.m_currentClipAmmo.ToString() + "x";
    }

    public void UpdateThrowableHUD()
    {
        m_WeaponHUDThrowableImgGO.GetComponent<Image>().sprite = m_playerWeaponSystem.CurrentThrowable.m_weaponSprite;
        for (int i = 1; i <= m_playerWeaponSystem.PlayerThrowableList.Count; i++)
        {
            string tempName = "Throwable" + i.ToString();
            Transform currentThrowableGO = m_WeaponHUDThrowablesContainerGO.transform.FindChild(tempName);
            if (currentThrowableGO == null)
            {
                currentThrowableGO = (Instantiate(m_WeaponHUDThrowablesContainerGO.transform.FindChild("Throwable1").gameObject, Vector3.zero, Quaternion.identity) as GameObject).transform;
                currentThrowableGO.SetParent(m_WeaponHUDThrowablesContainerGO.transform, false);
                currentThrowableGO.name = tempName;
            }

            currentThrowableGO.FindChild("Image").GetComponent<Image>().sprite = m_playerWeaponSystem.PlayerThrowableList[i - 1].m_weaponSprite;
            if (m_playerWeaponSystem.PlayerThrowableList[i - 1] == m_playerWeaponSystem.CurrentThrowable)
            {
                currentThrowableGO.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            }
            else
                currentThrowableGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void AnimateBlood()
    {
        StopCoroutine("AnimateBloodOnCR");
        StartCoroutine("AnimateBloodOnCR");
    }

    IEnumerator AnimateBloodOnCR()
    {
        Color currentColor = m_WeaponHUDBloodScreenImg.color;
        m_WeaponHUDBloodScreenImg.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a + 0.3f);
        float alpha = Mathf.Clamp(m_WeaponHUDBloodScreenImg.color.a, 0, 1);
        while (alpha > 0)
        {
            alpha -= 0.005f;
            m_WeaponHUDBloodScreenImg.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }

    public void ShowGameover()
    {
        m_gameoverGO.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnQuitBtnClicked()
    {
        Application.Quit();
    }

    public void OnQuitCancelBtnClicked()
    {
        m_quitPanelGO.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowToast(string msg,bool _override = false)
    {
        m_toastMsg = msg;
        if (!_override)
        {
            if (!m_toastGO.activeSelf)
                StartCoroutine("ShowToastCR");
        }
        else
        {
            m_toastGO.SetActive(false);
            StopCoroutine("ShowToastCR");
            StartCoroutine("ShowToastCR");
        }
    }

    IEnumerator ShowToastCR()
    {
        float tempTimer = m_toastGO.GetComponent<Animation>().clip.length;
        m_toastGO.SetActive(true);
        m_toastGO.transform.Find("Text").GetComponent<Text>().text = m_toastMsg;
        while (tempTimer > 0)
        {
            tempTimer -= Time.deltaTime;
            yield return null;
        }
        m_toastGO.SetActive(false);
    }
	
	public void ReloadScene()
    {
       SceneManager.LoadScene(0);
    }
}
