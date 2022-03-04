using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
//using UnityEngine.Networking;

public class GameManager : MonoBehaviour/*:NetworkBehaviour*/
{
    public static ActionDataBase actionDataBase = new ActionDataBase(0, 0, 0);
    public List<GameObject> cardsInRange;
    public List<GameObject> buttons, workList;
    public static readonly int[] startCredits = new int[] { 0, 1, 3, 5, 7, 9, 11, 13, 15 };
    //[SyncVar(hook = "SendHostData")]
    public int playerCount;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        PlayerPrefs.SetString("Name", "Ciamcia");
        PlayerPrefs.SetString("TypeOfDeck", GameCard.TypeOfDeck.Vuforians.ToString());
    }
    private void Start()
    {
        CheckManagers();
    }
    //private void Update()
    //{
    //    if (isServer)
    //    {
    //        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
    //            ActivateSceneObject(Input.GetTouch(0).position);
    //        else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
    //            ActivateSceneObject(Input.GetTouch(0).position);
    //        ////////Do usuniecia, po skoñczeniu projektu//////////////////////////////////////////////////////////////////////////
    //        if (Input.GetMouseButtonDown(0))
    //            ActivateSceneObject(Input.mousePosition);
    //        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //        if (!GameManager.actionDataBase.DefenderID.Equals(0))
    //            MakeAttack(GameManager.actionDataBase.ActionID);
    //    }
    //}
    //public void SendHostData(int playerCount)
    //{
    //    if (isServer && playerCount.Equals(2))
    //    {
    //        PlayerStatistics host = GameManager.FindNetworkGameObject(true, "Host").GetComponent<PlayerStatistics>();
    //        host.SetHostData();
    //    }
    //}
    //public void AddPlayer(PlayerStatistics playerStatistics)
    //{
    //    playerStatistics.playerID = playerCount < 1 ? playerCount : 1;
    //    playerStatistics.credits = GameManager.startCredits[GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round + 1];
    //}
    private void CheckManagers()
    {
        GameObject[] gameManagers = GameObject.FindGameObjectsWithTag("GameManager");
        if (gameManagers.Length > 1)
            for (int i = 1; i < gameManagers.Length; i++)
                Destroy(gameManagers[i]);
    }
    public void SetCardObject(GameObject card, int position, string field)
    {
        foreach (GameCard gameCard in GameCardsDataBase.GameCardList)
            if (card.GetComponent<CardStatistics>().id.Equals(gameCard.ID))
                switch (field)
                {
                    case "NewCard":
                        ChangeCardData(position, gameCard, gameCard.SetCost);
                        break;
                    case "ChangePosition":
                        ChangeCardData(position, gameCard, gameCard.MovingCost);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
    }
    public void ChangeCardData(int position, GameCard gameCard, int costField)
    {
        if (FindNetworkPlayer(gameCard.Deck, "Deck").Credits >= costField)
        {
            if (FindGameCardInScene(gameCard.ID, "ID").transform.GetChild(0).transform.GetChild(2).gameObject.activeSelf)
                ActivateObject(FindGameCardInScene(gameCard.ID, "ID").transform.GetChild(0).transform.GetChild(2).gameObject);
            PlayerData.PlayerStruct player = FindNetworkPlayer(gameCard.Deck, "Deck");
            player.Credits -= costField;
            gameCard.CardPosition = position;
            UpdateCardStatistics(gameCard, "CardPosition");
            UpdatePlayerCredits(FindNetworkPlayer(gameCard.Deck, "Deck").PlayerID);
        }
        else
        {
            if (FindGameCardInScene(gameCard.ID, "ID").transform.GetChild(0).transform.GetChild(2).gameObject.activeSelf.Equals(false))
                ActivateObject(FindGameCardInScene(gameCard.ID, "ID").transform.GetChild(0).transform.GetChild(2).gameObject);
        }
    }
    private void ActivateSceneObject(Vector3 inputPosition)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(inputPosition), out RaycastHit Hit) && Hit.transform.GetComponent<MeshRenderer>() /*&& Hit.transform.CompareTag("SpaceShip").*/ && CheckActionStatus(actionDataBase.ActionID, Hit.transform.gameObject))
        {
            if (actionDataBase.ActionID.Equals(0) && CheckActionConditions(actionDataBase.AttackerID, Hit.transform.gameObject))
                for (int i = 7; i < 9; i++)
                    ActivateObject(FindGameCardInScene(Hit.transform.name, "Name").transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).gameObject);
            SetID(FindGameCardInScene(Hit.transform.name, "Name"));
        }
        else if (!(Hit.transform is null))
            ActivateObject(Hit.transform.parent.transform.GetChild(2).gameObject);
    }
    private bool CheckActionConditions(int condition, GameObject gameObject) => condition switch
    {
        0 => true,
        _ => gameObject.transform.parent.transform.parent.transform.GetComponent<CardStatistics>().id.Equals(condition)
    };
    private bool CheckCredits(GameCard card) => card.Deck switch
    {
        GameCard.TypeOfDeck.Vuforians => FindNetworkPlayer(GameCard.TypeOfDeck.Vuforians, "Deck").Credits >= card.MovingCost,
        GameCard.TypeOfDeck.BlackSuns => FindNetworkPlayer(GameCard.TypeOfDeck.BlackSuns, "Deck").Credits >= card.MovingCost,
        _ => throw new ArgumentOutOfRangeException()
    };
    private dynamic CheckActionStatus(int condition, GameObject hittedGameObject) => condition switch
    {
        0 => CheckCredits(FindCardInDataBase(hittedGameObject.transform.parent.transform.parent.transform.GetComponent<CardStatistics>().id)),
        _ => true
    };
    public static void ActivateObject(GameObject gameObject)
    {
        gameObject.SetActive(SetBool(gameObject.activeSelf));
    }
    public static bool SetBool(bool trueOrFalse) => trueOrFalse switch
    {
        true => false,
        false => true,
    };
    private void SetID(GameObject gameObject)
    {
        SetAttacker(gameObject);
        SetDefender(gameObject);
    }
    private void SetAttacker(GameObject gameObject)
    {
        if (GameManager.actionDataBase.ActionID.Equals(0) && GameManager.actionDataBase.AttackerID.Equals(0))
            GameManager.actionDataBase.AttackerID = gameObject.GetComponent<CardStatistics>().id;
        else if (GameManager.actionDataBase.ActionID.Equals(0) && !GameManager.actionDataBase.AttackerID.Equals(0))
            GameManager.actionDataBase.AttackerID = 0;
    }
    public static void SetPotentialDefenders(List<GameObject> cardsInRange)
    {
        List<int> availabelPositions = actionDataBase.ActionID.Equals(1) ? AttackIsAvailable(FindCardInDataBase(actionDataBase.AttackerID).TypeOfWeapon.ToString(), FindCardInDataBase(actionDataBase.AttackerID).CardPosition) : AttackIsAvailable("Ram", FindCardInDataBase(actionDataBase.AttackerID).CardPosition);
        foreach (int availabelPosition in availabelPositions)
            if (FindGameCardInScene(availabelPosition, "CardPosition") != null)
            {
                FindGameCardInScene(availabelPosition, "CardPosition").GetComponent<CardStatistics>().inRange = true;
                cardsInRange.Add(FindGameCardInScene(availabelPosition, "CardPosition"));
            }
    }
    public static GameCard FindCardInDataBase(int id)
    {
        return GameCardsDataBase.GameCardList.Find(delegate (GameCard gameCard) { return gameCard.ID.Equals(id); });
    }
    public static GameObject FindGameCardInScene(dynamic condition, string field)
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        GameObject matchCard = Array.Find(cards, card => field switch
        {
            "CardPosition" => card.GetComponent<CardStatistics>().cardPosition.Equals(condition),
            "ID" => card.GetComponent<CardStatistics>().id.Equals(condition),
            "Attacker" => card.GetComponent<CardStatistics>().id.Equals(condition),
            "Defender" => card.GetComponent<CardStatistics>().id.Equals(condition),
            "Name" => card.transform.GetChild(0).transform.GetChild(0).name.Equals(condition),
            _ => throw new ArgumentOutOfRangeException()
        });
        return matchCard;
    }
    public static void UnsetInRange(List<GameObject> cardsInRange)
    {
        foreach (GameObject card in cardsInRange)
            card.GetComponent<CardStatistics>().inRange = false;
        cardsInRange.Clear();
    }
    public static dynamic AttackIsAvailable(dynamic weapon, int attackerPosition) => weapon switch
    {
        "Rackets" => CheckAvailabelPositions(GameCard.RacketsAvailabelPositions, attackerPosition),
        "Laser" => CheckAvailabelPositions(GameCard.CardPositionData, attackerPosition),
        "Kinetic" => CheckAvailabelPositions(GameCard.CardPositionData, attackerPosition),
        "Ram" => CheckAvailabelPositions(GameCard.CardPositionData, attackerPosition),
        _ => throw new ArgumentOutOfRangeException()
    };
    public static List<int> CheckAvailabelPositions(int[][] jaggedArray, int defaultPosition)
    {
        List<int> availabelPositions = new List<int>() { };
        for (int i = 0; i < jaggedArray[defaultPosition].Length; i++)
            availabelPositions.Add(jaggedArray[defaultPosition][i]);
        return availabelPositions;
    }
    private void SetDefender(GameObject gameObject)
    {
        if (!GameManager.actionDataBase.ActionID.Equals(0) && !gameObject.GetComponent<CardStatistics>().id.Equals(GameManager.actionDataBase.AttackerID) && gameObject.GetComponent<CardStatistics>().inRange && GameManager.actionDataBase.DefenderID.Equals(0))
            GameManager.actionDataBase.DefenderID = gameObject.GetComponent<CardStatistics>().id;
        else if (!GameManager.actionDataBase.ActionID.Equals(0) && !gameObject.GetComponent<CardStatistics>().id.Equals(GameManager.actionDataBase.AttackerID) && gameObject.GetComponent<CardStatistics>().inRange && !GameManager.actionDataBase.DefenderID.Equals(0))
            GameManager.actionDataBase.DefenderID = 0;
    }
    public static void SetAction(int actionID)
    {
        if (GameManager.actionDataBase.ActionID.Equals(0))
            GameManager.actionDataBase.ActionID = actionID;
        else
            GameManager.actionDataBase.ActionID = 0;
    }
    private void MakeAttack(int actionID)
    {
        Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
        FindActionShips(dictionary);
        dictionary.TryGetValue("Defender", out GameObject defender);
        dictionary.TryGetValue("Attacker", out GameObject attacker);
        switch (actionID)
        {
            case 1:
                AttackAction(defender, attacker);
                break;
            case 2:
                RamAction(defender, attacker);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void FindActionShips(Dictionary<string, GameObject> dictionary)
    {
        dictionary.Add("Attacker", FindGameCardInScene(actionDataBase.AttackerID, "Attacker"));
        dictionary.Add("Defender", FindGameCardInScene(actionDataBase.DefenderID, "Defender"));
    }
    private void AttackAction(GameObject defender, GameObject attacker)
    {
        SetObjectDamage(defender, attacker);
        CheckHP(defender, attacker);
    }
    private void CheckHP(GameObject defender, GameObject attacker)
    {
        if (FindCardInDataBase(GameManager.actionDataBase.AttackerID).HP.Equals(0) && FindCardInDataBase(GameManager.actionDataBase.DefenderID).HP.Equals(0))
            DestroyBothShips(defender, attacker);
        else if (FindCardInDataBase(GameManager.actionDataBase.AttackerID).HP.Equals(0) && !FindCardInDataBase(GameManager.actionDataBase.DefenderID).HP.Equals(0))
            DestroyOneShip(FindCardInDataBase(GameManager.actionDataBase.AttackerID), FindCardInDataBase(GameManager.actionDataBase.DefenderID));
        else if (!FindCardInDataBase(GameManager.actionDataBase.AttackerID).HP.Equals(0) && FindCardInDataBase(GameManager.actionDataBase.DefenderID).HP.Equals(0))
            DestroyOneShip(FindCardInDataBase(GameManager.actionDataBase.DefenderID), FindCardInDataBase(GameManager.actionDataBase.AttackerID));
        else
            UpdateBothShips(FindCardInDataBase(GameManager.actionDataBase.DefenderID), FindCardInDataBase(GameManager.actionDataBase.AttackerID));
    }
    private void SetObjectDamage(GameObject defender, GameObject attacker)
    {
        if (CheckAvailableDefence(defender.GetComponent<CardStatistics>().cardPosition, attacker.GetComponent<CardStatistics>().cardPosition).Equals(false))
            SetWeaponDamage(attacker.GetComponent<CardStatistics>().typeOfWeapon, defender, attacker);
        else
        {
            SetWeaponDamage(attacker.GetComponent<CardStatistics>().typeOfWeapon, defender, attacker);
            SetWeaponDamage(attacker.GetComponent<CardStatistics>().typeOfWeapon, attacker, defender);
        }
    }
    private bool CheckAvailableDefence(int defenderPosition, int attackerPosition)
    {
        bool isAvailable = CheckJaggedArray(GameCard.CardPositionData, defenderPosition, attackerPosition);
        return isAvailable;
    }
    private bool CheckJaggedArray(int[][] jaggedArray, int defaultPosition, int availabelPosition)
    {
        bool trueOrFalse = false;
        for (int i = 0; i < jaggedArray[defaultPosition].Length; i++)
            if (jaggedArray[defaultPosition][i].Equals(availabelPosition))
                trueOrFalse = true;
        return trueOrFalse;
    }
    private void SetWeaponDamage(GameCard.TypeOfWeapons weapon, GameObject defender, GameObject attacker)
    {
        switch (weapon)
        {
            case GameCard.TypeOfWeapons.Laser:
                SetDamage(defender.GetComponent<CardStatistics>().id, attacker.GetComponent<CardStatistics>().damage, 4, 1, 2);
                break;
            case GameCard.TypeOfWeapons.Rackets:
                SetDamage(defender.GetComponent<CardStatistics>().id, attacker.GetComponent<CardStatistics>().damage, 1, 2, 4);
                break;
            case GameCard.TypeOfWeapons.Kinetic:
                SetDamage(defender.GetComponent<CardStatistics>().id, attacker.GetComponent<CardStatistics>().damage, 2, 4, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void SetDamage(int defenderID, int damage, int deflectorScale, int armorScale, int hpScale)
    {
        foreach (GameCard card in GameCardsDataBase.GameCardList)
            if (card.ID.Equals(defenderID))
            {
                int i = 0;
                while (CheckAttackConditions(damage, i))
                {
                    damage = CalculateDamage(i, card, damage, deflectorScale, armorScale, hpScale);
                    i++;
                }
            }
    }
    private bool CheckAttackConditions(int damage, int i)
    {
        bool trueOrFalse = true;
        if (i.Equals(3) || damage.Equals(0))
            trueOrFalse = false;
        return trueOrFalse;
    }
    private int CalculateDamage(int i, GameCard card, int damage, int deflectorScale, int armorScale, int hpScale)
    {
        Dictionary<string, float> dictionary = new Dictionary<string, float>();
        float damageValue = 0;
        switch (i)
        {
            case 0:
                if (!card.DeflectorShield.Equals(0))
                {
                    CheckDamage(card.DeflectorShield, damage, deflectorScale, dictionary);
                    dictionary.TryGetValue("DefenderPoints", out float deflectorShieldValue);
                    card.DeflectorShield = (int)deflectorShieldValue;
                    dictionary.TryGetValue("AttackerPoints", out damageValue);
                    dictionary.Clear();
                }
                break;
            case 1:
                if (card.DeflectorShield.Equals(0) && !card.Armor.Equals(0))
                {
                    CheckDamage(card.Armor, damage, armorScale, dictionary);
                    dictionary.TryGetValue("DefenderPoints", out float armorValue);
                    card.Armor = (int)armorValue;
                    dictionary.TryGetValue("AttackerPoints", out damageValue);
                    dictionary.Clear();
                }
                break;
            case 2:
                if (card.DeflectorShield.Equals(0) && card.Armor.Equals(0) && !card.HP.Equals(0))
                {
                    CheckDamage(card.HP, damage, hpScale, dictionary);
                    dictionary.TryGetValue("DefenderPoints", out float hpValue);
                    card.HP = (int)hpValue;
                    dictionary.TryGetValue("AttackerPoints", out damageValue);
                    dictionary.Clear();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return (int)damageValue;
    }
    private void CheckDamage(float defenderPoints, float attackerPoints, float attackerScale, Dictionary<string, float> dictionary)
    {
        if (defenderPoints < attackerPoints / attackerScale)
        {
            dictionary.Add("DefenderPoints", 0);
            dictionary.Add("AttackerPoints", (attackerPoints / attackerScale - defenderPoints) * attackerScale);
        }
        else if (defenderPoints.Equals(attackerPoints / attackerScale))
        {
            dictionary.Add("DefenderPoints", 0);
            dictionary.Add("AttackerPoints", 0);
        }
        else if (defenderPoints > attackerPoints / attackerScale)
        {
            dictionary.Add("DefenderPoints", defenderPoints - attackerPoints / attackerScale);
            dictionary.Add("AttackerPoints", 0);
        }
    }
    private void RamAction(GameObject defender, GameObject attacker)
    {
        int defenderSegments = CheckCountAvailabelSegments(FindCardInDataBase(defender.GetComponent<CardStatistics>().id).ShipSegments, FindIntTypeOfShip(defender.GetComponent<CardStatistics>().typeOfShip)), attackerSegments = CheckCountAvailabelSegments(FindCardInDataBase(attacker.GetComponent<CardStatistics>().id).ShipSegments, FindIntTypeOfShip(attacker.GetComponent<CardStatistics>().typeOfShip)), comparedCSegments = attackerSegments - defenderSegments;
        switch (comparedCSegments)
        {
            case -3:
                SetRamDamage(FindCardInDataBase(attacker.GetComponent<CardStatistics>().id), FindCardInDataBase(defender.GetComponent<CardStatistics>().id), attackerSegments, defenderSegments);
                break;
            case -2:
                SetRamDamage(FindCardInDataBase(attacker.GetComponent<CardStatistics>().id), FindCardInDataBase(defender.GetComponent<CardStatistics>().id), attackerSegments, defenderSegments);
                break;
            case -1:
                SetRamDamage(FindCardInDataBase(attacker.GetComponent<CardStatistics>().id), FindCardInDataBase(defender.GetComponent<CardStatistics>().id), attackerSegments, defenderSegments);
                break;
            case 0:
                DestroyBothShips(defender, attacker);
                break;
            case 1:
                ////////////////////////////////////////////////////////////////// walnij ifa sprawdzaj¹cego czy mniejszy statek jest zablokowany kart¹ specjaln¹, w sensie nie spierdoli xd  ////////////////
                SetRamDamage(FindCardInDataBase(defender.GetComponent<CardStatistics>().id), FindCardInDataBase(attacker.GetComponent<CardStatistics>().id), defenderSegments, attackerSegments);
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                break;
            case 2:
                ////////////////////////////////////////////////////////////////// walnij ifa sprawdzaj¹cego czy mniejszy statek jest zablokowany kart¹ specjaln¹, w sensie nie spierdoli xd  ////////////////
                SetRamDamage(FindCardInDataBase(defender.GetComponent<CardStatistics>().id), FindCardInDataBase(attacker.GetComponent<CardStatistics>().id), defenderSegments, attackerSegments);
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                break;
            case 3:
                ////////////////////////////////////////////////////////////////// walnij ifa sprawdzaj¹cego czy mniejszy statek jest zablokowany kart¹ specjaln¹, w sensie nie spierdoli xd  ////////////////
                SetRamDamage(FindCardInDataBase(defender.GetComponent<CardStatistics>().id), FindCardInDataBase(attacker.GetComponent<CardStatistics>().id), defenderSegments, attackerSegments);
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void SetRamDamage(GameCard destroyedShip, GameCard survivedShip, int countTime, int segments)
    {
        ////////////////////////////////////////////////////////////////// walnij ifa sprawdzaj¹cego czy mniejszy statek jest zablokowany kart¹ specjaln¹, w sensie nie spierdoli xd a mo¿e nawet tutaj
        int i = 0;
        while (CheckCondition(countTime, ControledLength(survivedShip.ShipSegments[FindIntTypeOfShip(survivedShip.TypeOfShip)].Length), i))
        {
            if (!survivedShip.ShipSegments[FindIntTypeOfShip(survivedShip.TypeOfShip)][0, i].Equals(0))
            {
                survivedShip.ShipSegments[FindIntTypeOfShip(survivedShip.TypeOfShip)][0, i] = 0;
                survivedShip.ShipSegments[FindIntTypeOfShip(survivedShip.TypeOfShip)][1, i] = 0;
                countTime -= 1;
            }
            i = SetSegments(segments, i);
            i++;
        }
        int[,] newValues = CalculateShipSegments(survivedShip);
        survivedShip.HP = newValues[0, 0];
        survivedShip.Armor = newValues[1, 0];
        DestroyOneShip(destroyedShip, survivedShip);
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
    private bool CheckCondition(int countTime, int arrayLength, int i)
    {
        bool trueOrFalse = false;
        if (i > arrayLength || countTime > 0)
            trueOrFalse = true;
        return trueOrFalse;
    }
    private int SetSegments(int segments, int i) => segments switch
    {
        4 => i switch
        {
            0 => i += 2,
            3 => i -= 3,
            _ => i
        },
        _ => i
    };
    private int CheckCountAvailabelSegments(int[][,] shipSegments, int typeOfShip)
    {
        int availabelSegments = 0;
        for (int i = 0; i < ControledLength(shipSegments[typeOfShip].Length); i++)
            if (!shipSegments[typeOfShip][0, i].Equals(0))
                availabelSegments += 1;
        return availabelSegments;
    }
    public static int ControledLength(int length) => length switch
    {
        2 => 1,
        4 => 2,
        8 => 4,
        _ => throw new ArgumentOutOfRangeException()
    };
    public static int FindIntTypeOfShip(GameCard.TypeOfShips typeOfShips) => typeOfShips switch
    {
        GameCard.TypeOfShips.Corvette => 0,
        GameCard.TypeOfShips.Destroyer => 1,
        GameCard.TypeOfShips.Cruiser => 2,
        _ => throw new ArgumentOutOfRangeException()
    };
    public static int[,] CalculateShipSegments(GameCard gameCard)
    {
        int[,] value = new int[,] { { 0 }, { 0 } };
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < ControledLength(gameCard.ShipSegments[FindIntTypeOfShip(gameCard.TypeOfShip)].Length); j++)
                value[i, 0] += gameCard.ShipSegments[FindIntTypeOfShip(gameCard.TypeOfShip)][i, j];
        return value;
    }
    private void DestroyBothShips(GameObject defender, GameObject attacker)
    {
        GameCardsDataBase.GameCardList.Remove(FindCardInDataBase(GameManager.actionDataBase.DefenderID));
        GameCardsDataBase.GameCardList.Remove(FindCardInDataBase(GameManager.actionDataBase.AttackerID));
        UnsetInRange(cardsInRange);
        DestroyAllChildren(defender.transform.GetChild(0).gameObject);
        DestroyAllChildren(attacker.transform.GetChild(0).gameObject);
        ////efekty specjalne: wybuchy, poœcigi itp.itd.
        ActionDataBase.RemoveData(GameManager.actionDataBase);
        DeactivateButtons();
    }
    private void DestroyOneShip(GameCard destroyedShip, GameCard survivedShip)
    {
        GameCard.UpdateCardShipSegments(survivedShip.TypeOfShip, survivedShip.HP, survivedShip.Armor, survivedShip.ShipSegments);
        UpdateShip(survivedShip.ID);
        GameCardsDataBase.GameCardList.Remove(destroyedShip);
        UnsetInRange(cardsInRange);
        ButtonAction.ClearColor(buttons);
        DestroyAllChildren(FindGameCardInScene(destroyedShip.ID, "ID").transform.GetChild(0).gameObject);
        ////efekty specjalne: wybuchy, poœcigi itp.itd.
        ActionDataBase.RemoveData(GameManager.actionDataBase);
        DeactivateButtons();
    }
    private void UpdateBothShips(GameCard defender, GameCard attacker)
    {
        GameCard.UpdateCardShipSegments(defender.TypeOfShip, defender.HP, defender.Armor, defender.ShipSegments);
        GameCard.UpdateCardShipSegments(attacker.TypeOfShip, attacker.HP, attacker.Armor, attacker.ShipSegments);
        UpdateShip(GameManager.actionDataBase.AttackerID);
        UpdateShip(GameManager.actionDataBase.DefenderID);
        UnsetInRange(cardsInRange);
        ButtonAction.ClearColor(buttons);
        ////efekty specjalne: wybuchy, poœcigi itp.itd.
        ActionDataBase.RemoveData(GameManager.actionDataBase);
        DeactivateButtons();
    }
    private void UpdateShip(int shipID)
    {
        UpdateCardStatistics(FindCardInDataBase(shipID), "HP");
        UpdateCardStatistics(FindCardInDataBase(shipID), "Armor");
        UpdateCardStatistics(FindCardInDataBase(shipID), "Deflector");
    }
    private void DestroyAllChildren(GameObject objectBox)
    {
        GameObject card = objectBox.transform.parent.gameObject;
        objectBox.transform.parent = null;
        card.SetActive(false);
        while (!objectBox.transform.GetChild(1).transform.childCount.Equals(0))
        {
            GameObject gameObject = objectBox.transform.GetChild(1).transform.GetChild(0).gameObject;
            gameObject.transform.parent = null;
            if (gameObject.name.Equals("AttackButton") || gameObject.name.Equals("RamButton"))
                buttons.Remove(gameObject);
            Destroy(gameObject);
        }
        while (!objectBox.transform.childCount.Equals(0))
        {
            GameObject gameObject = objectBox.transform.GetChild(0).gameObject;
            gameObject.transform.parent = null;
            Destroy(gameObject);
        }
        Destroy(objectBox);
    }
    public static void UpdateCardStatistics(GameCard gameCard, string field)
    {
        SetField(FindGameCardInScene(gameCard.ID, "ID").GetComponent<CardStatistics>(), gameCard, field);
        UpdateCardText(FindGameCardInScene(gameCard.ID, "ID"));
    }
    public static void SetField(CardStatistics cardStatistics, GameCard gameCard, string field)
    {
        switch (field)
        {
            case "Armor":
                cardStatistics.armor = gameCard.Armor;
                break;
            case "HP":
                cardStatistics.hp = gameCard.HP;
                break;
            case "Deflector":
                cardStatistics.deflectorShield = gameCard.DeflectorShield;
                break;
            case "Damage":
                cardStatistics.damage = gameCard.Damage;
                break;
            case "CardPosition":
                cardStatistics.cardPosition = gameCard.CardPosition;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public static void UpdateCardText(GameObject card)
    {
        for (int i = 0; i < card.transform.GetChild(0).transform.GetChild(1).transform.childCount - 2; i++)
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = SetText(i, card);
    }
    private void DeactivateButtons()
    {
        foreach (GameObject button in buttons)
            if (button.activeSelf && !button.transform.parent.transform.parent.transform.parent.GetComponent<CardStatistics>().id.Equals(actionDataBase.AttackerID))
                button.SetActive(false);
    }
    private static string SetText(int i, GameObject card) => i switch
    {
        0 => card.GetComponent<CardStatistics>().cardName.ToString(),
        1 => "HP: " + card.GetComponent<CardStatistics>().hp.ToString(),
        2 => "Armor: " + card.GetComponent<CardStatistics>().armor.ToString(),
        3 => "Deflector: " + card.GetComponent<CardStatistics>().deflectorShield.ToString(),
        4 => "Damage: " + card.GetComponent<CardStatistics>().damage.ToString(),
        5 => "SetCost: " + card.GetComponent<CardStatistics>().setCost.ToString(),
        6 => "MovingCost: " + card.GetComponent<CardStatistics>().movingCost.ToString(),
        _ => throw new ArgumentOutOfRangeException()
    };
    public static void UpdatePlayerCredits(int playerID)
    {
        FindNetworkGameObject(playerID, "PlayerID").GetComponent<PlayerStatistics>().credits = FindNetworkPlayer(playerID, "PlayerID").Credits;
        UpdatePlayerText(playerID);
    }
    public static void UpdatePlayerText(int playerID)
    {
        /// do dopracowaniu przy ró¿nych widokach w apce, w sensie, ¿e po lewej dane w³asne, a po prawej przeciwnika ///////////////////////////////////////////////////
        GameObject.FindGameObjectWithTag("MainCanvas").transform.GetChild(playerID).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Credits: " + FindNetworkPlayer(playerID, "PlayerID").Credits.ToString();
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
    public static PlayerData.PlayerStruct FindNetworkPlayer(dynamic condition, string field)
    {
        PlayerData.PlayerStruct matchPlayer = new PlayerData.PlayerStruct();
        foreach (PlayerData.PlayerStruct player in PlayerDataBase.unSyncPlayersList)
            if (field.Equals("PlayerID") && player.PlayerID.Equals(condition))
                matchPlayer = player;
            else if (field.Equals("Deck") && player.TypeOfDeck.Equals(condition))
                matchPlayer = player;
        return matchPlayer;
    }
    public static GameObject FindNetworkGameObject(dynamic condition, string field)
    {
        GameObject[] networkPlayers = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        GameObject matchCard = Array.Find(networkPlayers, player => field switch
        {
            "PlayerID" => player.GetComponent<PlayerStatistics>().playerID.Equals(condition),
            "Deck" => player.GetComponent<PlayerStatistics>().typeOfDeck.Equals(condition),
            "Host" => player.GetComponent<PlayerStatistics>().host.Equals(condition),
            "Name" => player.GetComponent<PlayerStatistics>().playerName.Equals(condition),
            "NoName" => !player.GetComponent<PlayerStatistics>().playerName.Equals(condition),
            _ => throw new ArgumentOutOfRangeException()
        });
        return matchCard;
    }
    public static void SetCredits(int credits)
    {
        for (int i = 0; i < PlayerDataBase.unSyncPlayersList.Count; i++)
        {
            PlayerData.PlayerStruct player = PlayerDataBase.unSyncPlayersList[i];
            player.Credits += credits;
            UpdatePlayerCredits(player.PlayerID);
        }
    }
}