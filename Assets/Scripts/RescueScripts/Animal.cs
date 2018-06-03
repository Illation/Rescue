using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
	public float moveSpeed = 2f;		// The speed the enemy moves at.

	private SpriteRenderer ren;			// Reference to the sprite renderer.
	private Transform frontCheck;		// Reference to the position of the gameobject used for checking if something is in front.
    private Rigidbody2D rigBod;

    public RescueGameController.AnimalTypes TypeName = RescueGameController.AnimalTypes.ANIMAL_RABBIT;
    private List<Collider2D> CollidedWalls = new List<Collider2D>();

    public float MinDirChange = 2;
    public float MaxDirChange = 3;
    private float currDirChange = 0;

    public float MinStopTime = 2;
    public float MaxStopTime = 4;
    public float MinWalkTime = 1;
    public float MaxWalkTime = 3;
    private float currStopTime = 0;
    private bool hasStopped = true;

    private Transform groundCheck;          // A position marking where to check if the player is grounded.
    public Transform GroundCheck
    {
        get { return groundCheck; }
    }
    private bool grounded = false;          // Whether or not the player is grounded.
    public bool IsGrounded
    {
        get { return grounded; }
    }
    public float jumpForce = 1000f;

    public RescueGameController.BaitTypes BaitType = RescueGameController.BaitTypes.RABBIT_BAIT;
    public float VisibleBaitDist = 8f;
    public float CaptureBaitDist = 1f;
    private GameObject targetedBait = null;
    public enum BaitStates
    {
        UNAWARE,
        CHARMED,
        ON_BAIT
    };
    private BaitStates baitState = BaitStates.UNAWARE;
    private bool isTrapped = false;
    public bool IsTrapped
    {
        get { return isTrapped; }
    }

    private GameObject shock;
    private ParticleSystem hearts;

    // Stuff to make sure the zoo keeper spooks the animals
    public ZooKeeper Keeper;
    public float VisibleFleeDist = 4f;
    public float SpiderSense = 1.5f;
    private bool isFleeing = false;
    public float FleeCooldownTime = 1f;
    private float currFleeCDTime = 0;
    public float FleeSpeedMult = 1.5f;

    public bool FacingKeeper
    {
        get
        {
            return Facing(Keeper.transform);
        }
    }

    bool Facing(Transform trans)
    {
        if (transform.localScale.x < 0)
        {
            return trans.position.x < transform.position.x;
        }
        else
        {
            return trans.position.x > transform.position.x;
        }
    }

    void Awake()
	{
		// Setting up the references.
		ren = transform.Find("body").GetComponent<SpriteRenderer>();
		frontCheck = transform.Find("frontCheck").transform;
        rigBod = transform.GetComponent<Rigidbody2D>();

        shock = transform.Find("Shock").gameObject;
        shock.gameObject.SetActive(false);
        hearts = transform.Find("Hearts").GetComponent<ParticleSystem>();
        hearts.gameObject.SetActive(false);

        Keeper = GameObject.Find("ZooKeeper").GetComponent<ZooKeeper>();

        groundCheck = transform.Find("groundCheck");

        if (Random.Range(0, 2) == 1)
        {
            Flip();
        }

        currDirChange = Random.Range(MinDirChange, MaxDirChange);
        currStopTime = Random.Range(0f, MaxStopTime);
	}

    private void Update()
    {
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
    }

    void FixedUpdate ()
	{
		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);

		// Check each of the colliders.
		foreach(Collider2D c in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(c.tag == "Obstacle")
			{
				// ... Flip the enemy and stop checking the other colliders.
				Flip ();
				break;
			}
		}

        FleeingBehaviour();

        BaitingBehaviour();

        Move();
	}

    public void Move()
    {
        currStopTime -= Time.deltaTime;
        if (currStopTime <= 0 && IsGrounded)
        {
            hasStopped = !hasStopped;
            if (hasStopped)
            {
                currStopTime = Random.Range(MinStopTime, MaxStopTime);
            }
            else 
            {
                currStopTime = Random.Range(MinWalkTime, MaxWalkTime);
            }
        }
        if (baitState == BaitStates.ON_BAIT || (hasStopped && !isFleeing && !(baitState == BaitStates.CHARMED)))
        {
            currDirChange -= Time.deltaTime;
            if (currDirChange <= 0)
            {
                currDirChange = Random.Range(MinDirChange, MaxDirChange);
                Flip();
            }

            rigBod.velocity = new Vector2(0, rigBod.velocity.y);	
        }
        else
        {
            CheckAnimalBounds();
            float horizontalVel = transform.localScale.x * moveSpeed * (isFleeing ? FleeSpeedMult : 1);
            rigBod.velocity = new Vector2(horizontalVel, rigBod.velocity.y);	
            if(IsGrounded)
            {
                // Add a vertical force to the player.
                rigBod.AddForce(new Vector2(0f, jumpForce));
            }
        }
    }

    void CheckAnimalBounds()
    {
        foreach(var wallObj in CollidedWalls)
        {
            Wall wall = wallObj.GetComponent<Wall>();
            if(Facing(wall.transform))
            {
                Flip();
            }
        }
    }

    public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}

    void FleeingBehaviour()
    {
        float keeperDist = Vector3.Distance(transform.position, Keeper.transform.position);
        if(!FacingKeeper)
        {
            if(keeperDist < SpiderSense)
            {
                Flip();
            }
        }
        if(FacingKeeper && keeperDist < VisibleFleeDist)
        {
            Spook();
        }
        if(isFleeing)
        {
            if(FacingKeeper)
            {
                Flip();
            }
            currFleeCDTime -= Time.deltaTime;
            if(keeperDist < VisibleFleeDist)
            {
                currFleeCDTime = FleeCooldownTime;
            }
            if(currFleeCDTime<0)
            {
                isFleeing = false;
                shock.SetActive(false);
            }
        }
    }

    public void Spook()
    {
        if (isTrapped) return;
        if(FacingKeeper)
        {
            Flip();
        }
        isFleeing = true;
        shock.SetActive(true);
    }

    void BaitingBehaviour()
    {
        if(isFleeing)
        {
            baitState = BaitStates.UNAWARE;
            targetedBait = null;
            hearts.Stop();
            hearts.gameObject.SetActive(false);
            return;
        }
        switch(baitState)
        {
            case BaitStates.UNAWARE:
                targetedBait = FindBait();
                if(targetedBait)
                {
                    baitState = BaitStates.CHARMED;
                    hearts.gameObject.SetActive(true);
                    hearts.Play();
                }
                break;
            case BaitStates.CHARMED:
                if(!Facing(targetedBait.transform))
                {
                    Flip();
                }
                float trapDist = Mathf.Abs(targetedBait.transform.position.x - transform.position.x);
                if(trapDist < CaptureBaitDist)
                {
                    baitState = BaitStates.ON_BAIT;
                }
                break;
            case BaitStates.ON_BAIT:
                if(targetedBait.GetComponent<Trap>().IsTriggered)
                {
                    isTrapped = true;
                    targetedBait.GetComponent<Trap>().SetTrappedAnimal(this);
                }
                break;
        }
    }

    GameObject FindBait()
    {
        var traps = GameObject.FindGameObjectsWithTag("Trap");

        GameObject retTrap = null;

        float closestTrapDist = VisibleBaitDist;
        foreach (GameObject trap in traps)
        {
            Trap trapComp = trap.GetComponent<Trap>();
            if(trapComp)
            {
                bool facingTrap = Facing(trapComp.transform);
                if (trapComp.GetSelectedBait == BaitType && facingTrap && !trapComp.IsTriggered)
                {
                    float trapDist = Vector3.Distance(trap.transform.position, transform.position);
                    if(trapDist < closestTrapDist)
                    {
                        retTrap = trap;
                        closestTrapDist = trapDist;
                    }
                }
            }
        }

        return retTrap;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "AnimalWall")
        {
            if(collision.GetComponent<Wall>().AnimalType == TypeName)
            {
                CollidedWalls.Add(collision);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        CollidedWalls.Remove(collision);
    }
}
