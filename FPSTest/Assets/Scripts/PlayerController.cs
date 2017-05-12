using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private static PlayerController _instance;

    public GameObject m_playerHeadGO;
    public GameObject m_WeaponHUDImgGO;
    public GameObject m_WeaponHUDWeaponsContainerGO;
    public GameObject m_WeaponHUDAmmoTextGO;
    public GameObject m_WeaponHUDHealthTextGO;

    public float m_runSpeed = 1.0f;
    public float m_sprintMultiplier = 1;
    public float m_mouseSenstivity = 1.0f;
    public float m_jumpForce = 5.0f;

    public int m_totalHealth = 100;
    int m_currentHealth;

    bool m_isgrounded = false;

    float m_moveFB;
    float m_moveLR;

    float m_rotX;
    float m_rotY;

    Vector3 m_movement;

    Vector3 m_currentPlayerPos;
    Vector3 m_lastPlayerPos;

    WeaponSystemLogic m_weaponSystem;

    public static PlayerController Instance
    {
        get { return _instance; }
    }

    // Use this for initialization
    void Awake () {
        _instance = this;
        Cursor.visible = false;
        m_weaponSystem = this.GetComponent<WeaponSystemLogic>();
    }

    void Start()
    {
        SwitchWeapon(0);
        m_weaponSystem.ReloadAmmo();
        m_currentHealth = m_totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StopCoroutine("ShowMouseScope");
            StartCoroutine("ShowMouseScope");
        }
        if (Input.GetMouseButtonDown(0) && m_weaponSystem.CurrentWeapon().m_isShootable)
        {
            m_weaponSystem.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_weaponSystem.ReloadAmmo();
        }
    }

    void SwitchWeapon(int id)
    {
        m_weaponSystem.SwitchWeapon(id);
        UpdateWeaponHUD();
    }


	void FixedUpdate () {

        m_rotX = Input.GetAxis("Mouse X") * m_mouseSenstivity;
        m_rotY = Input.GetAxis("Mouse Y") * m_mouseSenstivity;

        transform.Rotate(0, m_rotX, 0);
        m_playerHeadGO.transform.Rotate(-m_rotY, 0, 0);
        Quaternion tempRot = m_playerHeadGO.transform.rotation;
        tempRot = new Quaternion(Mathf.Clamp(tempRot.x, -0.3f, 0.3f), tempRot.y, tempRot.z, tempRot.w);
        m_playerHeadGO.transform.rotation = tempRot;
        m_playerHeadGO.transform.eulerAngles = new Vector3(m_playerHeadGO.transform.eulerAngles.x, m_playerHeadGO.transform.eulerAngles.y, 0) ;

        if (m_isgrounded)
        {
            m_currentPlayerPos = this.transform.position;

            float tempMultiplier = 1;
            m_moveFB = Input.GetAxis("Vertical") * m_runSpeed;
            m_moveLR = Input.GetAxis("Horizontal") * m_runSpeed;

            m_movement = new Vector3(m_moveLR, 0, m_moveFB);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                tempMultiplier = m_sprintMultiplier;
            }

            if (Input.GetAxis("Jump") > 0)
            {
                Vector3 forwardForce = (m_currentPlayerPos - m_lastPlayerPos) * 30;
                Debug.Log(forwardForce);
                forwardForce = new Vector3(Mathf.Clamp(forwardForce.x, -0.3f, 0.3f), forwardForce.y, forwardForce.z);
                Debug.Log(forwardForce);
                this.GetComponent<Rigidbody>().AddForce((Vector3.up * m_jumpForce + forwardForce) * tempMultiplier , ForceMode.Impulse);
                Debug.Log("Jump");
            }
            m_movement *= tempMultiplier;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                m_movement *= m_sprintMultiplier;
            }
            this.transform.Translate(m_movement * Time.deltaTime);
            m_lastPlayerPos = m_currentPlayerPos;
        }
    }

    public void UpdateHealth(int amount)
    {
        m_currentHealth += amount;
        m_WeaponHUDHealthTextGO.GetComponent<Text>().text = string.Format("{0}/{1}", m_currentHealth.ToString(), m_totalHealth.ToString());
    }

    void UpdateWeaponHUD()
    {
        m_WeaponHUDImgGO.GetComponent<Image>().sprite = m_weaponSystem.CurrentWeapon().m_weaponSprite;
        for (int i = 1; i <= m_weaponSystem.m_weaponList.Count; i++)
        {
            string tempName = "Weapon" + i.ToString();
            Transform currentWeaponGO = m_WeaponHUDWeaponsContainerGO.transform.FindChild(tempName);
            if (currentWeaponGO == null)
            {
                currentWeaponGO = (Instantiate(m_WeaponHUDWeaponsContainerGO.transform.FindChild("Weapon1").gameObject, Vector3.zero, Quaternion.identity) as GameObject).transform;
                currentWeaponGO.parent = m_WeaponHUDWeaponsContainerGO.transform;
                currentWeaponGO.name = tempName;
            }

            currentWeaponGO.FindChild("Image").GetComponent<Image>().sprite = m_weaponSystem.m_weaponList[i - 1].m_weaponSprite;
            if(m_weaponSystem.m_weaponList[i - 1] == m_weaponSystem.CurrentWeapon())
            {
                currentWeaponGO.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            }
            else
                currentWeaponGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void UpdateAmmoAmountHUD()
    {
        m_WeaponHUDAmmoTextGO.GetComponent<Text>().text = string.Format("{0}/{1}", m_weaponSystem.CurrentWeapon().m_currentClipAmmo, m_weaponSystem.CurrentWeapon().m_totalAmmo);
    }


    IEnumerator ShowMouseScope()
    {
        m_weaponSystem.CurrentWeapon().m_weaponGO.GetComponent<Animator>().SetBool("Zoom", true);
        while (Input.GetMouseButton(1))
        {
          
            yield return null;
        }
        m_weaponSystem.CurrentWeapon().m_weaponGO.GetComponent<Animator>().SetBool("Zoom", false);
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.name == "Floor")
        {
            Invoke("SetIsGroundedTrue", 0.2f);
        }
    }

    void SetIsGroundedTrue()
    {
        m_isgrounded = true;
    }

    void OnCollisionExit(Collision coll)
    {
        if (coll.transform.name == "Floor")
        {
            m_isgrounded = false;
        }
    }
}
