using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvArea : MonoBehaviour {

    private bool playerInArea = false;
    public bool IsPlayerInArea
    {
        get { return playerInArea; }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            print("Entering " + name);
            playerInArea = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            print("Leaving " + name);
            playerInArea = false;
        }
    }
}
