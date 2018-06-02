using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public GameObject SetSprite;
    public GameObject ActivatedSprite;

	// Use this for initialization
	void Start ()
    {
        SetSprite.SetActive(true);
        ActivatedSprite.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Activate()
    {
        SetSprite.SetActive(false);
        ActivatedSprite.SetActive(true);
    }
}
