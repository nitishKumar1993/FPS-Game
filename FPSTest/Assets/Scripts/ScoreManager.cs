using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    private static ScoreManager _instance;
    public Text m_HUDScoreTextGO;

    int m_totalScore = 0;

    public static ScoreManager Instance
    {
        get { return _instance; }
    }

    public void AddScore(int value)
    {
        m_totalScore += value;
        UpdateScoreHUD();
    }

    void UpdateScoreHUD()
    {
        m_HUDScoreTextGO.text = m_totalScore.ToString();
    }

	// Use this for initialization
	void Start () {
        _instance = this;
        UpdateScoreHUD();

    }
}
