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
        ANIMAL_BADGER,
        ANIMAL_PONY,
        ANIMAL_SALMON,
        ANIMAL_GOOSE,
        ANIMAL_HERON,
        ANIMAL_WOMBAT,
        NUM_ANIMALS
    };

    public enum Enviroments
    {
        ENVIROMENT_FIELD,
        ENVIROMENT_LAKE
    }

    public enum TrapType
    {
        TRAPTYPE_SURFACE,
        TRAPTYPE_UNDERSURFACE
    }




    public enum BaitTypes
    {
        NONE_BAIT,
        PORPOSISE_BAIT,
        RABBIT_BAIT,
        BADGER_BAIT,
        PONY_BAIT,
        SALMON_BAIT,
        GOOSE_BAIT,
        HERON_BAIT,
        WOMBAT_BAIT,
    }

    public enum GameStates
    {
        GOTO_ENV,
        GET_TRAP,
        BAIT_TRAP,
        CAPTURE_ANIMAL,
        WIN
    };


    public List<GameObject> Baits;
    public PlayerControl Player;
    public ZooKeeper zookeeper;

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

    public List<EnvArea> EnvTriggers;
    private EnvArea EnvTriggerToUse;
    private TrapType wantedTrapType;
    public TrapType WantedTrapType
    {
        get { return wantedTrapType;  }
    }

    private Dictionary<AnimalTypes, Enviroments> AnimalEnviroments;
    private Dictionary<AnimalTypes, TrapType> AnimalTrapTypes;
    private Dictionary<AnimalTypes, BaitTypes> AnimalBaitTypes;

    private Dictionary<KeyValuePair<TrapType, Enviroments>, List<BaitTypes>> BaitForEachTrap;

    private BaitTypes neededBaitType;
    public BaitTypes NeededBaitType
    {
        get { return neededBaitType;  }
    }

    private Enviroments enviromentPlayerIn;
    public Enviroments EnviromentPlayerIn
    {
        get { return enviromentPlayerIn; }
    }

    // Use this for initialization
    void Start () {
        s_this = this;

        gameState = GameStates.GOTO_ENV;

        InstructionList = new Dictionary<GameStates, string>();
        InstructionList.Add(GameStates.GOTO_ENV, "Goto where the animal might be found.");
        InstructionList.Add(GameStates.GET_TRAP, "Go and get the right type of trap.");
        InstructionList.Add(GameStates.BAIT_TRAP, "Place the trap and bait it.");
        InstructionList.Add(GameStates.CAPTURE_ANIMAL, "Trigger the trap when the animal is close.");
        InstructionList.Add(GameStates.WIN, "Well done - you got the correct animal.");

        Clue1List = new Dictionary<AnimalTypes, string>();
        Clue1List.Add(AnimalTypes.ANIMAL_PORPOISE, "I like the water.");
        Clue1List.Add(AnimalTypes.ANIMAL_RABBIT, "Give me space to roam.");
        Clue1List.Add(AnimalTypes.ANIMAL_BADGER, "Give me space to roam.");
        Clue1List.Add(AnimalTypes.ANIMAL_PONY, "Give me space to roam.");
        Clue1List.Add(AnimalTypes.ANIMAL_SALMON, "I like the water.");
        Clue1List.Add(AnimalTypes.ANIMAL_GOOSE, "I like the water.");
        Clue1List.Add(AnimalTypes.ANIMAL_HERON, "I like the water.");
        Clue1List.Add(AnimalTypes.ANIMAL_WOMBAT, "Give me space to roam.");

        Clue2List = new Dictionary<AnimalTypes, string>();
        Clue2List.Add(AnimalTypes.ANIMAL_PORPOISE, "Swim, swim, swim.");
        Clue2List.Add(AnimalTypes.ANIMAL_RABBIT, "Give me space!.");
        Clue2List.Add(AnimalTypes.ANIMAL_BADGER, "I like to sleep with a roof over my head.");
        Clue2List.Add(AnimalTypes.ANIMAL_PONY, "Give me space!.");
        Clue2List.Add(AnimalTypes.ANIMAL_SALMON, "Swim, swim, swim.");
        Clue2List.Add(AnimalTypes.ANIMAL_GOOSE, "On and over, but not under.");
        Clue2List.Add(AnimalTypes.ANIMAL_HERON, "On and over, but not under.");
        Clue2List.Add(AnimalTypes.ANIMAL_WOMBAT, "I like to sleep with a roof over my head.");

        Clue3List = new Dictionary<AnimalTypes, string>();
        Clue3List.Add(AnimalTypes.ANIMAL_PORPOISE, "Always happy, always smiling.");
        Clue3List.Add(AnimalTypes.ANIMAL_RABBIT, "Big ears, big everything.");
        Clue3List.Add(AnimalTypes.ANIMAL_BADGER, "Black and white and underground.");
        Clue3List.Add(AnimalTypes.ANIMAL_PONY, "I'm a miniature powerhouse.");
        Clue3List.Add(AnimalTypes.ANIMAL_SALMON, "I travel home to have children.");
        Clue3List.Add(AnimalTypes.ANIMAL_GOOSE, "Honk!");
        Clue3List.Add(AnimalTypes.ANIMAL_HERON, "Strutting in the shallows.");
        Clue3List.Add(AnimalTypes.ANIMAL_WOMBAT, "G'day mate!");

        AnimalEnviroments = new Dictionary<AnimalTypes, Enviroments>();
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_PORPOISE, Enviroments.ENVIROMENT_LAKE);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_RABBIT, Enviroments.ENVIROMENT_FIELD);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_BADGER, Enviroments.ENVIROMENT_FIELD);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_PONY, Enviroments.ENVIROMENT_FIELD);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_SALMON, Enviroments.ENVIROMENT_LAKE);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_GOOSE, Enviroments.ENVIROMENT_LAKE);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_HERON, Enviroments.ENVIROMENT_LAKE);
        AnimalEnviroments.Add(AnimalTypes.ANIMAL_WOMBAT, Enviroments.ENVIROMENT_FIELD);

        AnimalTrapTypes = new Dictionary<AnimalTypes, TrapType>();
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_PORPOISE, TrapType.TRAPTYPE_UNDERSURFACE);
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_RABBIT, TrapType.TRAPTYPE_SURFACE);
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_BADGER, TrapType.TRAPTYPE_UNDERSURFACE);
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_PONY, TrapType.TRAPTYPE_SURFACE);
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_SALMON, TrapType.TRAPTYPE_UNDERSURFACE);
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_GOOSE, TrapType.TRAPTYPE_SURFACE);
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_HERON, TrapType.TRAPTYPE_SURFACE);
        AnimalTrapTypes.Add(AnimalTypes.ANIMAL_WOMBAT, TrapType.TRAPTYPE_UNDERSURFACE);

        AnimalBaitTypes = new Dictionary<AnimalTypes, BaitTypes>();
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_PORPOISE, BaitTypes.PORPOSISE_BAIT);
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_RABBIT, BaitTypes.RABBIT_BAIT);
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_BADGER, BaitTypes.BADGER_BAIT);
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_PONY, BaitTypes.PONY_BAIT);
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_SALMON, BaitTypes.SALMON_BAIT);
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_GOOSE, BaitTypes.GOOSE_BAIT);
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_HERON, BaitTypes.HERON_BAIT);
        AnimalBaitTypes.Add(AnimalTypes.ANIMAL_WOMBAT, BaitTypes.WOMBAT_BAIT);

        BaitForEachTrap = new Dictionary<KeyValuePair<TrapType, Enviroments>, List<BaitTypes>>();
        BaitForEachTrap.Add(new KeyValuePair<TrapType, Enviroments>(TrapType.TRAPTYPE_SURFACE, Enviroments.ENVIROMENT_FIELD), new List<BaitTypes>() { BaitTypes.PONY_BAIT, BaitTypes.RABBIT_BAIT });
        BaitForEachTrap.Add(new KeyValuePair<TrapType, Enviroments>(TrapType.TRAPTYPE_UNDERSURFACE, Enviroments.ENVIROMENT_FIELD), new List<BaitTypes>() { BaitTypes.BADGER_BAIT, BaitTypes.WOMBAT_BAIT });
        BaitForEachTrap.Add(new KeyValuePair<TrapType, Enviroments>(TrapType.TRAPTYPE_SURFACE, Enviroments.ENVIROMENT_LAKE), new List<BaitTypes>() { BaitTypes.HERON_BAIT, BaitTypes.GOOSE_BAIT });
        BaitForEachTrap.Add(new KeyValuePair<TrapType, Enviroments>(TrapType.TRAPTYPE_UNDERSURFACE, Enviroments.ENVIROMENT_LAKE), new List<BaitTypes>() { BaitTypes.PORPOSISE_BAIT, BaitTypes.SALMON_BAIT });

        targetAnimal = (AnimalTypes)Random.Range(0, (int)AnimalTypes.NUM_ANIMALS);
        EnvTriggerToUse = EnvTriggers[(int)AnimalEnviroments[targetAnimal]];
        wantedTrapType = AnimalTrapTypes[targetAnimal];
        neededBaitType = AnimalBaitTypes[targetAnimal];
    }
	
	// Update is called once per frame
	void Update ()
    {
        InstructionText.text = InstructionList[gameState];

        switch (gameState)
        {
            case GameStates.GOTO_ENV:
                ClueText.text = Clue1List[targetAnimal];
                if (EnvTriggerToUse.IsPlayerInArea)
                {
                    gameState = GameStates.GET_TRAP;
                }
                break;

            case GameStates.GET_TRAP:
                ClueText.text = Clue2List[targetAnimal];
                if (zookeeper.TrapInstance != null && zookeeper.TrapInstance.trapType == wantedTrapType)
                {
                    gameState = GameStates.BAIT_TRAP;
                }
                break;

            case GameStates.BAIT_TRAP:
                ClueText.text = Clue3List[targetAnimal];
                if (zookeeper.TrapInstance != null && zookeeper.TrapInstance.IsBaited() && zookeeper.TrapInstance.GetSelectedBait == neededBaitType)
                {
                    gameState = GameStates.CAPTURE_ANIMAL;
                }
                break;

            case GameStates.CAPTURE_ANIMAL:
                ClueText.text = "";
                if (zookeeper.TrapInstance != null && zookeeper.TrapInstance.TrappedAnimal != null && zookeeper.TrapInstance.TrappedAnimal.TypeName == targetAnimal)
                {
                    gameState = GameStates.WIN;
                }
                break;

            case GameStates.WIN:
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

    public List<BaitTypes> GetBaitsForTrap(TrapType trapType, Enviroments enviroment)
    {
        return BaitForEachTrap[new KeyValuePair<TrapType, Enviroments>(trapType, enviroment)];
    }

    public void PlayerInEnviroment(Enviroments enviroment)
    {
        enviromentPlayerIn = enviroment;
    }
}
