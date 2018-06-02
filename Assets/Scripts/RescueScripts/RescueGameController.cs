using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueGameController : MonoBehaviour {

    public enum BaitTypes
    {
        NONE_BAIT,
        PORPOSISE_BAIT,
        RABBIT_BAIT
    }

    public List<GameObject> Baits;
    public PlayerControl Player;

    static RescueGameController s_this;
    static public RescueGameController Instance
    {
        get { return s_this;  }
    }

    // Use this for initialization
    void Start () {
        s_this = this;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetBaitPrefab(BaitTypes BaitType)
    {
        return Baits[(int)BaitType];
    }

    public void AllowPlayerToMove(bool val)
    {
        Player.enabled = val;
    }
}
