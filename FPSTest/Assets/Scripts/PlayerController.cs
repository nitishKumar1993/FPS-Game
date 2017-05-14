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
    public Image m_WeaponHUDBloodScreenImg;

    public GameObject m_gameoverGO;

    public float m_runSpeed = 1.0f;
    public float m_sprintMultiplier = 1;
    public float m_mouseSenstivity = 1.0f;
    public float m_jumpForce = 5.0f;

    int m_totalHealth = 100;
    int m_currentHealth;

    bool m_isgrounded = false;
    bool m_isPlayerDead = false;
    bool m_isScoped = false;

    float m_moveFB;
    float m_moveLR;

    float m_rotX;
    float m_rotY;

    public WeaponSystemLogic m_weaponSystem;
    

    public static PlayerController Instance
    {
        get { return _instance; }
    }

    public int CurrentHealth
    {
        get { return m_currentHealth; }
    }

    public bool IsPlayerDead
    {
        get { return m_isPlayerDead; }
    }

    // Use this for initialization
    void Awake () {
        _instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_weaponSystem = this.GetComponent<WeaponSystemLogic>();
    }

    void Start()
    {
        m_playerHeadGO.transform.eulerAngles = Vector3.zero;
        SwitchWeapon(0);
        m_weaponSystem.ReloadAmmo();
        m_currentHealth = m_totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isPlayerDead)
        {
            if (Input.GetMouseButtonDown(1))
            {
                StopCoroutine("ShowMouseScope");
                StartCoroutine("ShowMouseScope");
            }
            if (Input.GetMouseButtonDown(0) && m_weaponSystem.CurrentWeapon.m_isShootable)
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
            else if (Input.GetKeyDown(KeyCode.F))
            {
                m_weaponSystem.AttackMelee();
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                int weaponID = m_weaponSystem.CurrentWeaponID + 1;
                if (weaponID == m_weaponSystem.m_weaponList.Count)
                    weaponID = 0;
                SwitchWeapon(weaponID);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                int weaponID = m_weaponSystem.CurrentWeaponID - 1;
                if (weaponID < 0)
                    weaponID = m_weaponSystem.m_weaponList.Count - 1;
                SwitchWeapon(weaponID);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                m_weaponSystem.ReloadAmmo();
            }
        }
    }

    void SwitchWeapon(int id)
    {
        m_weaponSystem.SwitchWeapon(id);
        UpdateWeaponHUD();
    }


	void FixedUpdate () {
        if (!m_isPlayerDead)
        {
            m_rotX = Input.GetAxis("Mouse X") * m_mouseSenstivity * (m_isScoped ? 0.5f : 1);
            m_rotY = Input.GetAxis("Mouse Y") * m_mouseSenstivity * (m_isScoped ? 0.5f : 1);

            transform.Rotate(0, m_rotX, 0);
            m_playerHeadGO.transform.Rotate(-m_rotY, 0, 0);
            Quaternion tempRot = m_playerHeadGO.transform.rotation;
            tempRot = new Quaternion(Mathf.Clamp(tempRot.x, -0.2f, 0.3f), tempRot.y, tempRot.z, tempRot.w);
            m_playerHeadGO.transform.rotation = tempRot;
            m_playerHeadGO.transform.localEulerAngles = new Vector3(m_playerHeadGO.transform.localEulerAngles.x, 0, 0);

            if (m_isgrounded)
            {
                float tempMultiplier = 1;
                m_moveFB = Input.GetAxis("Vertical") * m_runSpeed;
                m_moveLR = Input.GetAxis("Horizontal") * m_runSpeed;

                Vector3 m_movement = new Vector3(m_moveLR, 0, m_moveFB);

                if (Mathf.Abs(m_moveFB) > 0 || Mathf.Abs(m_moveLR) > 0)
                {
                    if (!m_playerAlreadyAnimating)
                        StartCoroutine("AnimatePlayer");
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    m_weaponSystem.StopReload();
                    m_playerHeadGO.GetComponent<Animator>().speed = 2;
                    tempMultiplier = m_sprintMultiplier;
                }
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    m_playerHeadGO.GetComponent<Animator>().speed = 1;
                }

                if (Input.GetAxis("Jump") > 0)
                {
                    Vector3 forwardForce = ((Vector3.forward * m_moveFB) / 3 + (Vector3.right * m_moveLR) / 3)* tempMultiplier;
                    m_isgrounded = false;
                    this.GetComponent<Rigidbody>().AddRelativeForce((Vector3.up * m_jumpForce + forwardForce) * tempMultiplier, ForceMode.Impulse);
                }
                m_movement *= tempMultiplier;

                this.transform.Translate(m_movement * Time.deltaTime,Space.Self);
            }
        }
    }


    bool m_playerAlreadyAnimating = false;
    IEnumerator AnimatePlayer()
    {
        m_playerAlreadyAnimating = true;
        m_playerHeadGO.GetComponent<Animator>().SetBool("Run",true);
        while (Mathf.Abs(m_moveFB) > 0 || Mathf.Abs(m_moveLR) > 0 && !m_isPlayerDead)
        {
            if (!m_isScoped && m_isgrounded)
            {
                m_playerHeadGO.GetComponent<Animator>().SetBool("Run", true);
            }
            else
            {
                m_playerHeadGO.GetComponent<Animator>().SetBool("Run", false);
            }
            yield return null;
        }
        m_playerHeadGO.GetComponent<Animator>().speed = 1;
        m_playerAlreadyAnimating = false;
        m_playerHeadGO.GetComponent<Animator>().SetBool("Run", false);
    }

    public void OnPlayerDamage(int amount)
    {
        UpdateHealth(-amount);

        StopCoroutine("AnimateBloodOnScreen");
        StartCoroutine("AnimateBloodOnScreen");
    }

    public void HealPlayer(int amount)
    {
        UpdateHealth(amount);
    }

    IEnumerator AnimateBloodOnScreen()
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

    public void UpdateHealth(int amount)
    {
        if(m_currentHealth <= m_totalHealth)
            m_currentHealth += amount;
        m_currentHealth = Mathf.Clamp(m_currentHealth,0, m_totalHealth);
        m_WeaponHUDHealthTextGO.GetComponent<Text>().text = string.Format("{0}/{1}", m_currentHealth.ToString(), m_totalHealth.ToString());
        if (CurrentHealth <= 0)
        {
            ShowGameover();
        }
        else if (CurrentHealth <= m_totalHealth * 0.3f)
        {
            GameManager.Instance.ShowToast("Health Low");
        }
    }

    void ShowGameover()
    {
        m_gameoverGO.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        m_playerHeadGO.GetComponent<Animator>().enabled = false;
        m_isPlayerDead = true;
    }

    void UpdateWeaponHUD()
    {
        m_WeaponHUDImgGO.GetComponent<Image>().sprite = m_weaponSystem.CurrentWeapon.m_weaponSprite;
        for (int i = 1; i <= m_weaponSystem.m_weaponList.Count; i++)
        {
            string tempName = "Weapon" + i.ToString();
            Transform currentWeaponGO = m_WeaponHUDWeaponsContainerGO.transform.FindChild(tempName);
            if (currentWeaponGO == null)
            {
                currentWeaponGO = (Instantiate(m_WeaponHUDWeaponsContainerGO.transform.FindChild("Weapon1").gameObject, Vector3.zero, Quaternion.identity) as GameObject).transform;
                currentWeaponGO.SetParent(m_WeaponHUDWeaponsContainerGO.transform,false);
                currentWeaponGO.name = tempName;
            }

            currentWeaponGO.FindChild("Image").GetComponent<Image>().sprite = m_weaponSystem.m_weaponList[i - 1].m_weaponSprite;
            if(m_weaponSystem.m_weaponList[i - 1] == m_weaponSystem.CurrentWeapon)
            {
                currentWeaponGO.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            }
            else
                currentWeaponGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void UpdateAmmoAmountHUD()
    {
        m_WeaponHUDAmmoTextGO.GetComponent<Text>().text = string.Format("{0}/{1}", m_weaponSystem.CurrentWeapon.m_currentClipAmmo, m_weaponSystem.CurrentWeapon.m_extraAmmo);
    }


    IEnumerator ShowMouseScope()
    {
        m_isScoped = true;
        m_weaponSystem.CurrentWeapon.m_weaponGO.GetComponent<Animator>().SetBool("Zoom", true);
        while (Input.GetMouseButton(1))
        {
            if (m_playerHeadGO.GetComponent<Camera>().fieldOfView > 40)
            {
                m_playerHeadGO.GetComponent<Camera>().fieldOfView -= 4;
            }

            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                break;
            }
            yield return null;
        }
        while (m_playerHeadGO.GetComponent<Camera>().fieldOfView < 60)
        {
            m_playerHeadGO.GetComponent<Camera>().fieldOfView += 5;
        }
        m_isScoped = false;
        m_weaponSystem.CurrentWeapon.m_weaponGO.GetComponent<Animator>().SetBool("Zoom", false);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.name == "Floor" || coll.transform.tag == "AI" || coll.transform.tag == "Obstacle")
        {
            Ray ray = new Ray(this.transform.position, coll.contacts[0].point - this.transform.position);
            RaycastHit hitInfo;

            Debug.DrawRay(this.transform.position, coll.contacts[0].point - this.transform.position);

            if (Physics.Raycast(ray, out hitInfo, 1))
            {
                if (!m_isgrounded)
                {
                    Invoke("SetIsGroundedTrue", 0.2f);
                }
            }
        }
    }

    void SetIsGroundedTrue()
    {
        m_isgrounded = true;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
