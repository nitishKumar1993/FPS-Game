using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private static PlayerController _instance;

    public GameObject m_playerHeadGO;
    public Transform m_muzzleTransform;
    public GameObject m_bulletPrefab;

    public float m_runSpeed = 1.0f;
    public float m_sprintMultiplier = 1;
    public float m_mouseSenstivity = 1.0f;
    public float m_jumpForce = 5.0f;
    public Animator m_weaponAnimator;

    bool m_isgrounded = false;

    float m_moveFB;
    float m_moveLR;

    float m_rotX;
    float m_rotY;

    Vector3 m_movement;

    float m_shootInterval = 0.1f;

    Vector3 m_currentPlayerPos;
    Vector3 m_lastPlayerPos;


    public static PlayerController Instance
    {
        get { return _instance; }
    }

    // Use this for initialization
    void Awake () {
        _instance = this;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Mouse button pressed");
            StopCoroutine("ShowMouseScope");
            StartCoroutine("ShowMouseScope");
        }
        if (Input.GetMouseButtonDown(0))
        {
            StopCoroutine("Shoot");
            StartCoroutine("Shoot");
        }
    }

    IEnumerator Shoot()
    {
        while(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.375f, 0));
            GameObject tempBullet = Instantiate(m_bulletPrefab, m_muzzleTransform.position, Quaternion.identity) as GameObject;
            tempBullet.transform.LookAt(ray.GetPoint(500));

            yield return new WaitForSeconds(m_shootInterval);
        }
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

    IEnumerator ShowMouseScope()
    {
        m_weaponAnimator.SetBool("Zoom", true);
        while (Input.GetMouseButton(1))
        {
          
            yield return null;
        }
        m_weaponAnimator.SetBool("Zoom", false);
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
