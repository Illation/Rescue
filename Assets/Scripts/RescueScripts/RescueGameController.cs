using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RescueGameController : MonoBehaviour
{
    public enum AnimalTypes
    {
        ANIMAL_PORPOISE,
        ANIMAL_RABBIT,
        NUM_ANIMALS
    };

    public enum Enviroments
    {
        ENVIROMENT_FIELD,
        ENVIROMENT_LAKE
    }




    public enum BaitTypes
    {
        NONE_BAIT,
        PORPOSISE_BAIT,
        RABBIT_BAIT
    }

    public enum GameStates
    {
        GOTO_ENV,
        GET_TRAP,
        BAIT_TRAP,
        CAPTURE_ANIMAL
    };


    public List<GameObject> Baits;
    public PlayerControl Player;

    public Text ClueText;
    public Text InstructionText;

    static RescueGameController s_this;
    static public RescueGameController Instance
    {
        get { return s_this;  }
    }

    private AnimalTypes targetAnimal;
    private GameStates gameState;

    private Dictionary<GameStates, string> InstructionList;

    private Dictionary<AnimalTypes, string> Clue1List;
    private Dictionary<AnimalTypes, string> Clue2List;
    private Dictionary<AnimalTypes, string> Clue3List;

    public List<BoxCollider2D> EnvTriggers;
    private BoxCollider2D EnvTriggerToUse;

    private Dictionary<AnimalTypes, Enviroments> AnimalEnviroments;

    // Use this for initialization
    void Start () {
        s_this = this;

        gameState = GameStates.GOTO_ENV;

        InstructionList = new Dictionary<GameStates, string>();
        InstructionList.Add(GameStates.GOTO_ENV, "Goto where the animal might be found.");
        InstructionList.Add(GameStates.GET_TRAP, "Go and get the right type of trap.");
        InstructionList.Add(GameStates.BAIT_TRAP, "Place the trap and bait it.");
        InstructionList.Add(GameStates.CAPTURE_ANIMAL, "Trigger the trap when the animal is close.");

        Clue1List = new Dictionary<AnimalTypes, string>();
        Clue1List.Add(AnimalTypes.ANIMAL_PORPOISE, "I like the water.");
        Clue1List.Add(AnimalTypes.ANIMAL_RABBIT, "Give me space to roam.");

        Clue2List = new Dictionary<AnimalTypes, string>();
        Clue2List.Add(AnimalTypes.ANIMAL_PORPOISE, "Swim, swim, swim.");
        Clue2List.Add(AnimalTypes.ANIMAL_RABBIT, "Give me space!.");

        Clue3List = new Dictionary<AnimalTypes, string>();
        Clue3List.Add(AnimalTypes.ANIMAL_PORPOISE, "Always happy, always smiling.");
        Clue3List.Add(AnimalTypes.ANIMAL_RABBIT, "Big ears, big everything.");

        AnimalEnviroments = new Dictionary<AnimalTypes, Enviroments>();
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_PORPOISE, Enviroments.ENVIROMENT_LAKE);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_RABBIT, Enviroments.ENVIROMENT_FIELD);

        targetAnimal = (AnimalTypes)Random.Range(0, (int)AnimalTypes.NUM_ANIMALS);
        EnvTriggerToUse = EnvTriggers[(int)AnimalEnviroments[targetAnimal]];
    }
	
	// Update is called once per frame
	void Update ()
    {
        InstructionText.text = InstructionList[gameState];

        switch (gameState)
        {
            case GameStates.GOTO_ENV:
                ClueText.text = Clue1List[targetAnimal];
                break;

            case GameStates.GET_TRAP:
                ClueText.text = Clue2List[targetAnimal];
                break;

            case GameStates.BAIT_TRAP:
                ClueText.text = Clue2List[targetAnimal];
                break;

            case GameStates.CAPTURE_ANIMAL:
                ClueText.text = "";
                break;
        };
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
