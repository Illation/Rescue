using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitMenuItem : MonoBehaviour {

    public GameObject SelectedGO;

	// Use this for initialization
	void Start () {
        SelectedGO.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSelected(bool sel)
    {
        SelectedGO.SetActive(sel);
    }
}
