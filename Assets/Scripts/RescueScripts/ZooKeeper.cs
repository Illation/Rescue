using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZooKeeper : MonoBehaviour
{
    public PlayerControl thePlayerControl;
    public Trap trapPrefab;
    

    private bool dropTrap = false;
    private bool activateTrap = false;

    private bool holdingTrap = false;
    private bool trapSet = false;

    private Trap trapCloseTo;
    private Trap trapInstance;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("DropTrap") && thePlayerControl.IsGrounded && holdingTrap)
        {
            dropTrap = true;
        }

        if (Input.GetButtonDown("PickupTrap") && thePlayerControl.IsGrounded && holdingTrap)
        {
            dropTrap = true;
        }

        if (Input.GetButtonDown("ActivateTrap") && trapSet)
        {
            activateTrap = true;
        }


    }

    private void FixedUpdate()
    {
        if (dropTrap)
        {
            print("DropTrap");
            trapInstance = Instantiate(trapPrefab);
            trapInstance.transform.position = thePlayerControl.GroundCheck.position;
            dropTrap = false;
            holdingTrap = false;
            trapSet = true;
        }

        if (activateTrap)
        {
            trapInstance.Activate();
            trapSet = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Trap")
        {
            print("Close to trap");
            trapCloseTo = col.GetComponent<Trap>();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Trap")
        {
            print("Leaving trap");
            trapCloseTo = null;
        }
    }


}
