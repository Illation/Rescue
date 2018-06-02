using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{
	public float moveSpeed = 2f;		// The speed the enemy moves at.

	private SpriteRenderer ren;			// Reference to the sprite renderer.
	private Transform frontCheck;		// Reference to the position of the gameobject used for checking if something is in front.
    private Rigidbody2D rigBod;

    public float MinDirChange = 2;
    public float MaxDirChange = 3;
    private float currDirChange = 0;

    public float MinStopTime = 2;
    public float MaxStopTime = 4;
    public float MinWalkTime = 1;
    public float MaxWalkTime = 3;
    private float currStopTime = 0;
    private bool hasStopped = true;

	
	void Awake()
	{
		// Setting up the references.
		ren = transform.Find("body").GetComponent<SpriteRenderer>();
		frontCheck = transform.Find("frontCheck").transform;
        rigBod = transform.GetComponent<Rigidbody2D>();

        currDirChange = MinDirChange;
        currStopTime = MinStopTime;
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

        currDirChange -= Time.deltaTime;
        if(currDirChange <= 0)
        {
            currDirChange = Random.Range(MinDirChange, MaxDirChange);
            Flip();
        }

        Move();
	}

    public void Move()
    {
        currStopTime -= Time.deltaTime;
        if (currStopTime <= 0)
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
        if (hasStopped)
        {
            rigBod.velocity = new Vector2(0, rigBod.velocity.y);	
        }
        else
        {
            rigBod.velocity = new Vector2(transform.localScale.x * moveSpeed, rigBod.velocity.y);	
        }
    }

    public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}
}
