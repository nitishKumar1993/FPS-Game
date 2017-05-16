using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour {
    private static WeaponsManager _instance;

    public List<Weapon> m_weaponList;
    public List<Weapon> m_throwableList;
    public List<Weapon> m_meleeList;

    public static WeaponsManager Instance
    {
        get { return _instance; }
    }

    // Use this for initialization
    void Awake () {
        _instance = this;
	}
}
