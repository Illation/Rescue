using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZooKeeper : MonoBehaviour
{
    public PlayerControl thePlayerControl;
    public Trap trapPrefab;


    private bool pickupTrap = false;
    private bool dropTrap = false;
    private bool activateTrap = false;
    private bool baitTrap = false;

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
        if (trapCloseTo == null || !trapCloseTo.IsChoosingBait())
        {
            if (Input.GetButtonDown("DropTrap") && thePlayerControl.IsGrounded && holdingTrap)
            {
                dropTrap = true;
            }
            else if (Input.GetButtonDown("PickupTrap") && thePlayerControl.IsGrounded && trapCloseTo != null)
            {
                pickupTrap = true;
                trapSet = false;
            }
            else if (Input.GetButtonDown("ActivateTrap") && trapSet)
            {
                activateTrap = true;
            }
            else if (Input.GetButtonDown("BaitTrap") && trapCloseTo != null && !trapCloseTo.IsChoosingBait() && trapCloseTo.IsBaited() == false)
            {
                baitTrap = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (dropTrap)
        {
            print("DropTrap");
            if (trapInstance != null)
            {
                trapInstance.transform.position = thePlayerControl.GroundCheck.position;
                trapInstance.gameObject.SetActive(true);
                dropTrap = false;
                holdingTrap = false;
                trapSet = true;
            }
        }

        if (pickupTrap)
        {
            if (trapCloseTo != null)
            {
                trapInstance = trapCloseTo;
                trapCloseTo.gameObject.SetActive(false);
                trapCloseTo = null;
                holdingTrap = true;
            }

            pickupTrap = false;
        }

        if (activateTrap)
        {
            trapInstance.Activate();
            trapSet = false;
            trapInstance = null;

            activateTrap = false;
        }

        if (baitTrap)
        {
            trapCloseTo.ShowBaitMenu();
            baitTrap = false;
            GetComponent<PlayerControl>().Stop();
            GetComponent<PlayerControl>().enabled = false;
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
