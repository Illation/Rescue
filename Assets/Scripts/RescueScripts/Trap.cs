using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public RescueGameController.TrapType trapType;
    public GameObject SetSprite;
    public GameObject ActivatedSprite;
    public BoxCollider2D PoximityTrigger;
    public GameObject BaitMenuParent;

    private List<RescueGameController.BaitTypes> ShownBaitTypes;
    private List<BaitMenuItem> ShownBaitObjs;

    int SelectedBaitIndex = 0;
    bool CanSelectionMove = false;
    bool CanMakeSelection = false;

    private Animal trappedAnimal;
    public Animal TrappedAnimal
    {
        get { return trappedAnimal;  }
    }

    RescueGameController.BaitTypes selectedBait = RescueGameController.BaitTypes.NONE_BAIT;
    public RescueGameController.BaitTypes GetSelectedBait
    {
        get { return selectedBait; }
    }

    public bool IsTriggered
    {
        get { return ActivatedSprite.gameObject.activeInHierarchy; }
    }

    // Use this for initialization
    void Start ()
    {
        SetSprite.SetActive(true);
        ActivatedSprite.SetActive(false);
        BaitMenuParent.SetActive(false);

        ShownBaitTypes = new List<RescueGameController.BaitTypes>();
        ShownBaitObjs = new List<BaitMenuItem>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (BaitMenuParent.activeInHierarchy)
        {
            for (int i = 0; i < ShownBaitObjs.Count; ++i)
            {
                ShownBaitObjs[i].SetSelected(i == SelectedBaitIndex);
            }

            if (Input.GetAxis("Horizontal") < 0 && CanSelectionMove)
            {
                --SelectedBaitIndex;
                if (SelectedBaitIndex < 0)
                {
                    SelectedBaitIndex = ShownBaitObjs.Count - 1;
                }

                CanSelectionMove = false;
            }
            if (Input.GetAxis("Horizontal") > 0 && CanSelectionMove)
            {
                ++SelectedBaitIndex;
                if (SelectedBaitIndex > ShownBaitObjs.Count - 1)
                {
                    SelectedBaitIndex = 0;
                }

                CanSelectionMove = false;
            }

            if (Input.GetAxis("Horizontal") == 0)
            {
                CanSelectionMove = true;
            }

            if (!Input.GetButtonDown("BaitTrap"))
            {
                CanMakeSelection = true;
            }

            if (Input.GetButtonDown("BaitTrap"))
            {
                selectedBait = ShownBaitTypes[SelectedBaitIndex];
                BaitMenuParent.SetActive(false);
                RescueGameController.Instance.AllowPlayerToMove(true);
            }
        }

	}

    public void Activate()
    {
        SetSprite.SetActive(false);
        ActivatedSprite.SetActive(true);
        PoximityTrigger.enabled = false;
    }

    public void ShowBaitMenu()
    {
        print("ShowBaitMenu");
        ShownBaitTypes.Clear();
        foreach (Transform child in BaitMenuParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        AddBaitType(RescueGameController.BaitTypes.PORPOSISE_BAIT);
        AddBaitType(RescueGameController.BaitTypes.RABBIT_BAIT);

        BaitMenuParent.SetActive(true);

        float diff = 3f;
        float startX = - ShownBaitObjs.Count * diff / 4;
        for (int i = 0; i < ShownBaitObjs.Count; ++i)
        {
            GameObject o = ShownBaitObjs[i].gameObject;
            o.transform.localPosition = new Vector3(startX, 1, 0);
            startX += diff;
        }

        CanSelectionMove = false;
        CanMakeSelection = false;



    }

    private void AddBaitType(RescueGameController.BaitTypes BaitType)
    {
        GameObject BaitObj = Instantiate(RescueGameController.Instance.Baits[(int)BaitType]);
        BaitObj.transform.parent = BaitMenuParent.transform;
        BaitObj.transform.transform.localPosition = new Vector3(0, 1, 0);
        ShownBaitObjs.Add(BaitObj.GetComponent<BaitMenuItem>());
        ShownBaitTypes.Add(BaitType);
    }

    public bool IsBaited()
    {
        return selectedBait != RescueGameController.BaitTypes.NONE_BAIT;
    }

    public bool IsChoosingBait()
    {
        return BaitMenuParent.activeInHierarchy;
    }

    public void SetTrappedAnimal(Animal animal)
    {
        trappedAnimal = animal;
    }
}
