using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public GameObject m_toastGO;

    string m_toastMsg = "";

    public static GameManager Instance
    {
        get { return _instance; }
    }

	// Use this for initialization
	void Awake () {
        _instance = this;
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
