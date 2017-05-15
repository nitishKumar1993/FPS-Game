using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private static PlayerController _instance;

    public float m_runSpeed = 1.0f;
    public float m_sprintMultiplier = 1;
    public float m_mouseSenstivity = 1.0f;
    public float m_jumpForce = 5.0f;

    public GameObject m_playerHeadGO;

    int m_totalHealth = 100;
    int m_currentHealth;

    bool m_isgrounded = false;
    bool m_isPlayerDead = false;
    bool m_isScoped = false;

    float m_moveFB;
    float m_moveLR;

    float m_rotX;
    float m_rotY;

    WeaponSystemLogic m_weaponSystem;
    
    public WeaponSystemLogic PlayerWeaponSystem
    {
        get { return m_weaponSystem; }
    }

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
        SwitchThrowable(0);
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

            if (Input.GetMouseButtonDown(2))
            {
                m_weaponSystem.UseThrowable();
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
              
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
              
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
    }

    void SwitchThrowable(int id)
    {
        m_weaponSystem.SwitchThrowable(id);
    }

    public void OnWeaponSwitched()
    {
        GameManager.Instance.UpdateWeaponHUD();
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
        GameManager.Instance.AnimateBlood();
    }

    public void HealPlayer(int amount)
    {
        UpdateHealth(amount);
    }

    public void UpdateHealth(int amount)
    {
        if(m_currentHealth <= m_totalHealth)
            m_currentHealth += amount;
        m_currentHealth = Mathf.Clamp(m_currentHealth,0, m_totalHealth);
        GameManager.Instance.HUDHealth(m_currentHealth, m_totalHealth);
        if (CurrentHealth <= 0)
        {
            m_playerHeadGO.GetComponent<Animator>().enabled = false;
            m_isPlayerDead = true;
            GameManager.Instance.ShowGameover();
        }
        else if (CurrentHealth <= m_totalHealth * 0.3f)
        {
            GameManager.Instance.ShowToast("Health Low");
        }
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
                    Invoke("SetIsGroundedTrue", 0.1f);
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
