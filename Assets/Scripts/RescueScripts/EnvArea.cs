using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvArea : MonoBehaviour {

    private bool playerInArea = false;
    public bool IsPlayerInArea
    {
        get { return playerInArea; }
    }

    public RescueGameController.Enviroments enviroment;

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
            RescueGameController.Instance.PlayerInEnviroment(enviroment);
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
