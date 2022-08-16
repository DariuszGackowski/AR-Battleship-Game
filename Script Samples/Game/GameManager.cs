using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [NonSerialized] public List<GameObject> cardsInRange = new List<GameObject>();
    [NonSerialized] public List<GameObject> playerList = new List<GameObject>();
    [NonSerialized] public List<GameObject> gameCardList = new List<GameObject>();
    [NonSerialized] public List<int> availabelPositions = new List<int>();

    [NonSerialized] public GameObject[] networkPlayers;

    [SerializeField] private GameObject playerPrefab;

    public static readonly int[] startCredits = new int[] { 0, 1, 3, 5, 7, 9, 10 };

    [Header ("Game Informations")]
    [Space] public TypeOfDeck typeOfDeck;
    public int attackerID, defenderID, playerInitiativeNumber, round, initiative;

    public enum TypeOfWeapons { Laser, Rackets, Kinetic, None };
    public enum TypeOfShips { Fighter, CargoShip };
    public enum TypeOfDeck { Vuforians, RedSuns };

    [NonSerialized] public bool playersAreSet;

    private new PhotonView photonView;


    public static int[][] cardPositionData = new int[4][]
    {
        new int[] {},
        new int[] {1,2}, // pole 1
        new int[] {1,2,3}, // pole 2
        new int[] {2,3} // pole 3
    };

    public static int[][] racketsAvailabelPositions = new int[4][]
    {
        new int[] {},
        new int[] {1,2,3},
        new int[] {1,2,3},
        new int[] {1,2,3}
    };


    private void Awake()
    {
        instance = this;

        Application.targetFrameRate = 60;

        CheckManagers();

        photonView = this.gameObject.GetComponent<PhotonView>();

        typeOfDeck = (GameManager.TypeOfDeck)Enum.Parse(typeof(GameManager.TypeOfDeck), PlayerPrefs.GetString("TypeOfDeck"));
    }

    private void Start() => StartGame(ChoiceContainer.decision);

    private void Update() => SetNetworkPlayers();
    private void SetNetworkPlayers()
    {
        if (!playersAreSet && playerList.Count.Equals(2))
        {
            networkPlayers = playerList.ToArray();
            playersAreSet = !playersAreSet;
        }
    }
    private void StartGame(int decision)
    {
        switch (decision)
        {
            case 1:
                playerInitiativeNumber = 1;
                break;
            case 2:
                playerInitiativeNumber = 2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GameObject newGameObject = PhotonNetwork.Instantiate(playerPrefab.name, this.transform.position, Quaternion.identity);
        newGameObject.name = newGameObject.GetComponent<Player>().playerName = typeOfDeck.ToString();
        newGameObject.GetComponent<Player>().typeOfDeck = typeOfDeck;
        newGameObject.GetComponent<Player>().playerID = playerInitiativeNumber;
        newGameObject.GetComponent<Player>().credits = 3;
        newGameObject.GetComponent<Player>().isMine = true;

        Destroy(GameObject.FindGameObjectWithTag("ChoiceContainer"));
    }

    private void CheckManagers()
    {
        GameObject[] gameManagers = GameObject.FindGameObjectsWithTag("GameManager");

        if (gameManagers.Length > 1)
            for (int i = 1; i < gameManagers.Length; i++)
                Destroy(gameManagers[i]);
    }

    public void SetCardPosition(GameObject card, int boxPosition, string field)
    {
        GameCard gameCard = card.GetComponent<GameCard>();

        switch (field)
        {
            case "NewCard":
                if (boxPosition.Equals(2))
                {
                    ChangeCardPosition(boxPosition, gameCard.id, gameCard.setCost + gameCard.movingCost); // naliczenie op³aty za po³o¿enie karty i jej przemieszczenie w przypadku po³o¿enia jej na œrodkowym polu
                }
                else
                {
                    ChangeCardPosition(boxPosition, gameCard.id, gameCard.setCost);
                }
                break;
            case "ChangePosition":
                ChangeCardPosition(boxPosition, gameCard.id, gameCard.movingCost);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void ChangeCardPosition(int boxPosition, int cardID, int costField)
    {
        GameObject card = FindGameCardInScene(cardID, "ID");
        GameCard gameCard = card.GetComponent<GameCard>();
        GameObject player = FindNetworkPlayer(gameCard.deck, "Deck");

        if (player.GetComponent<Player>().credits >= costField && !gameCard.used)
        {
            player.GetComponent<Player>().credits -= costField;
            gameCard.cardPosition = boxPosition;

            card.GetComponent<AudioSource>().Play();

            card.transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(true); // uaktywnienie "podkreœlenia" statku

            photonView.RPC("SyncPositionData", RpcTarget.All, boxPosition, gameCard.id, player.GetComponent<Player>().credits);
        }
        else
        {
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(true); /// uaktywnienie znaku zakazu
        }
    }
    public void ActivateSceneObject(GameObject card)
    {
        if (attackerID.Equals(0) || !attackerID.Equals(0) && card.GetComponent<GameCard>().id.Equals(attackerID))
        {
            SetAttacker(card);
        }
        else if (!attackerID.Equals(0))
        {
            SetDefender(card);
        }
    }

    public static void DeactivateButtons()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");

        foreach (GameObject card in cards)
        {
            GameObject button1 = card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject;
            GameObject button2 = card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(3).gameObject;

            ShipButton.ClearColor(button1);//czyszczenie kolorów buttonów\
            ShipButton.ClearColor(button2);//czyszczenie kolorów buttonów\
        }
    }
    public static bool CheckCredits(GameObject card) => card.GetComponent<GameCard>().deck switch
    {
        GameManager.TypeOfDeck.Vuforians => FindNetworkPlayer(GameManager.TypeOfDeck.Vuforians, "Deck").GetComponent<Player>().credits >= card.GetComponent<GameCard>().movingCost,
        GameManager.TypeOfDeck.RedSuns => FindNetworkPlayer(GameManager.TypeOfDeck.RedSuns, "Deck").GetComponent<Player>().credits >= card.GetComponent<GameCard>().movingCost,
        _ => throw new ArgumentOutOfRangeException()
    };
    private void SetAttacker(GameObject attacker)
    {
        if (attackerID.Equals(0))
        {
            photonView.RPC("SyncAttacker", RpcTarget.All, attacker.GetComponent<GameCard>().id);

            attackerID = attacker.GetComponent<GameCard>().id;

            SetPotentialDefenders();
        }
        else
        {
            instance.UnsetInRange();

            photonView.RPC("SyncAttacker", RpcTarget.All, 0);
        }
    }
    private void SetDefender(GameObject defender)
    {
        if (defenderID.Equals(0))
        {
            photonView.RPC("SyncDefender", RpcTarget.All, defender.GetComponent<GameCard>().id);
            photonView.RPC("SyncPlayerCredits", RpcTarget.All, attackerID);
            photonView.RPC("SyncMakeAttack", RpcTarget.All);
        }

    }
    public static void SetUnchecked()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");

        foreach (GameObject card in cards)
        {
            card.GetComponent<GameCard>().used = false;
            card.GetComponent<GameCard>().inRange = false;
        }
    }
    public void UpdatePlayerText()
    {
        GameObject mainCanva = GameObject.FindGameObjectWithTag("MainCanvas");

        GameObject creditsInfo1 = mainCanva.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject;
        GameObject creditsInfo2 = mainCanva.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).gameObject;

        creditsInfo1.GetComponent<TextMeshProUGUI>().text = "Credits: " + GameUserInterface.SetUIPlayerCredits("Left");
        creditsInfo2.GetComponent<TextMeshProUGUI>().text = "Credits: " + GameUserInterface.SetUIPlayerCredits("Right");
    }
    public void SetPotentialDefenders()
    {
        GameCard attackerCard = FindGameCardInScene(attackerID, "ID").GetComponent<GameCard>();

        availabelPositions = AttackIsAvailable(attackerCard.typeOfWeapon.ToString(), attackerCard.cardPosition);

        foreach (int availabelPosition in availabelPositions)
        {
            GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");

            foreach (GameObject card in cards)
            {
                GameCard gameCard = card.GetComponent<GameCard>();

                if (gameCard.cardPosition.Equals(availabelPosition))
                {
                    gameCard.inRange = true;

                    instance.cardsInRange.Add(card);
                }
            }
        }
    }
    public static dynamic AttackIsAvailable(dynamic weapon, int attackerPosition) => weapon switch
    {
        "Rackets" => CheckAvailabelPositions(GameManager.racketsAvailabelPositions, attackerPosition),
        "Laser" => CheckAvailabelPositions(GameManager.cardPositionData, attackerPosition),
        "Kinetic" => CheckAvailabelPositions(GameManager.cardPositionData, attackerPosition),
        _ => throw new ArgumentOutOfRangeException()
    };
    public static List<int> CheckAvailabelPositions(int[][] jaggedArray, int attackerPosition)
    {
        List<int> availabelPositions = new List<int>() { };

        for (int i = 0; i < jaggedArray[attackerPosition].Length; i++)
        {
            availabelPositions.Add(jaggedArray[attackerPosition][i]);
        }

        return availabelPositions;
    }
    public static GameObject FindGameCardInScene(dynamic condition, string field)
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        GameObject matchCard = Array.Find(cards, card => field switch
        {
            "CardPosition" => card.GetComponent<GameCard>().cardPosition.Equals(condition),
            "ID" => card.GetComponent<GameCard>().id.Equals(condition),
            "Attacker" => card.GetComponent<GameCard>().id.Equals(condition),
            "Defender" => card.GetComponent<GameCard>().id.Equals(condition),
            "Name" => card.transform.GetChild(0).transform.GetChild(0).name.Equals(condition),
            _ => throw new ArgumentOutOfRangeException()
        });
        return matchCard;
    }
    public void UnsetInRange()
    {
        foreach (GameObject card in instance.cardsInRange)
        {
            card.GetComponent<GameCard>().inRange = false;
        }

        cardsInRange.Clear();
    }
    private void AttackAction(GameObject defender, GameObject attacker)
    {
        SetObjectDamage(defender, attacker);
    }
    private void CheckHP(GameObject defender, GameObject attacker)
    {
        GameCard attackerCard = attacker.GetComponent<GameCard>();
        GameCard defenderCard = defender.GetComponent<GameCard>();

        if (attackerCard.hp.Equals(0) && defenderCard.hp.Equals(0))
        {
            DestroyBothShips(defender, attacker);
        }
        else if (attackerCard.hp.Equals(0) && !defenderCard.hp.Equals(0))
        {
            DestroyOneShip(attacker, defender);
        }
        else if (!attackerCard.hp.Equals(0) && defenderCard.hp.Equals(0))
        {
            DestroyOneShip(defender, attacker);
        }
        else
        {
            UpdateBothShips(defender, attacker);
        }
    }
    private void SetObjectDamage(GameObject defender, GameObject attacker)
    {
        GameCard defenderCard = defender.GetComponent<GameCard>();
        GameCard attackerCard = attacker.GetComponent<GameCard>();

        if (!CheckAvailableDefence(defenderCard.cardPosition, attackerCard.cardPosition))
        {
            StartShootEffect(attacker);
            StartHitEffect(defender);

            StartCoroutine(WaitSecondsSetDamage(2f, attackerCard.typeOfWeapon, defenderCard, attackerCard));

            StartCoroutine(WaitSecondsSetHP(2.5f, attacker, defender));
        }
        else
        {
            StartShootEffect(attacker);
            StartHitEffect(defender);

            StartCoroutine(WaitSecondsSetDamage(2f, attackerCard.typeOfWeapon, defenderCard, attackerCard));

            StartShootEffect(defender);
            StartHitEffect(attacker);

            StartCoroutine(WaitSecondsSetDamage(2f, defenderCard.typeOfWeapon, attackerCard, defenderCard));

            StartCoroutine(WaitSecondsSetHP(2.5f, attacker, defender));
        }
    }
    private IEnumerator WaitSecondsSetDamage(float timeInSeconds, TypeOfWeapons typeOfWeapon, GameCard defenderCard, GameCard attackerCard)
    {
        yield return new WaitForSeconds(timeInSeconds);

        SetWeaponDamage(typeOfWeapon, defenderCard, attackerCard);
    }
    private IEnumerator WaitSecondsSetHP(float timeInSeconds, GameObject attacker, GameObject defender)
    {
        yield return new WaitForSeconds(timeInSeconds);

        CheckHP(defender, attacker);
    }
    private bool CheckAvailableDefence(int defenderPosition, int attackerPosition)
    {
        bool isAvailable = CheckJaggedArray(GameManager.cardPositionData, defenderPosition, attackerPosition);
        return isAvailable;
    }
    private bool CheckJaggedArray(int[][] jaggedArray, int defenderPosition, int attackerPosition)
    {
        bool trueOrFalse = false;
        for (int i = 0; i < jaggedArray[attackerPosition].Length; i++)
            if (jaggedArray[attackerPosition][i].Equals(defenderPosition))
                trueOrFalse = true;
        return trueOrFalse;
    }
    private void SetWeaponDamage(GameManager.TypeOfWeapons weapon, GameCard defenderCard, GameCard attackerCard)
    {
        switch (weapon)
        {
            case GameManager.TypeOfWeapons.Laser:
                {
                    SetDamage(defenderCard.id, attackerCard.damage, 2, 1, 2);
                }
                break;
            case GameManager.TypeOfWeapons.Rackets:
                {
                    SetDamage(defenderCard.id, attackerCard.damage, 2, 2, 1);
                }
                break;
            case GameManager.TypeOfWeapons.Kinetic:
                {
                    SetDamage(defenderCard.id, attackerCard.damage, 1, 2, 2);
                }
                break;
            case GameManager.TypeOfWeapons.None:
                // do nothing
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void SetDamage(int defenderID, int damage, int deflectorScale, int armorScale, int hpScale)
    {
        foreach (GameObject card in gameCardList)
            if (card.GetComponent<GameCard>().id.Equals(defenderID))
            {
                int i = 0;
                while (CheckAttackConditions(damage, i))//koñczy pêtle kiedy i=3 lub damage = 0;
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
        {
            trueOrFalse = false;
        }

        return trueOrFalse;
    }
    private int CalculateDamage(int i, GameObject card, int damage, int deflectorScale, int armorScale, int hpScale)
    {
        Dictionary<string, float> dictionary = new Dictionary<string, float>();

        GameCard gameCard = card.GetComponent<GameCard>();

        float damageValue;

        switch (i)
        {
            case 0:
                if (!gameCard.deflectorShield.Equals(0))
                {
                    CheckDamage(gameCard.deflectorShield, damage, deflectorScale, dictionary);

                    dictionary.TryGetValue("DefenderPoints", out float deflectorShieldValue);

                    gameCard.deflectorShield = (int)deflectorShieldValue;

                    dictionary.TryGetValue("AttackerPoints", out damageValue);
                    dictionary.Clear();
                }
                else
                {
                    damageValue = damage;
                }
                break;
            case 1:
                if (gameCard.deflectorShield.Equals(0) && !gameCard.armor.Equals(0))
                {
                    CheckDamage(gameCard.armor, damage, armorScale, dictionary);

                    dictionary.TryGetValue("DefenderPoints", out float armorValue);


                    gameCard.armor = (int)armorValue;

                    dictionary.TryGetValue("AttackerPoints", out damageValue);
                    dictionary.Clear();
                }
                else
                {
                    damageValue = damage;
                }
                break;
            case 2:
                if (gameCard.deflectorShield.Equals(0) && gameCard.armor.Equals(0) && !gameCard.hp.Equals(0))
                {
                    CheckDamage(gameCard.hp, damage, hpScale, dictionary);

                    dictionary.TryGetValue("DefenderPoints", out float hpValue);

                    gameCard.hp = (int)hpValue;

                    dictionary.TryGetValue("AttackerPoints", out damageValue);
                    dictionary.Clear();
                }
                else
                {
                    damageValue = damage;
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
    private void DestroyBothShips(GameObject defender, GameObject attacker)
    {
        UnsetInRange();

        gameCardList.Remove(defender);
        gameCardList.Remove(attacker);
        
        DestroyAllChildren(defender.transform.GetChild(0).gameObject);
        DestroyAllChildren(attacker.transform.GetChild(0).gameObject);

        RemoveData();
    }
    private void DestroyOneShip(GameObject destroyedShip, GameObject survivedShip)
    {
        CheckWin(destroyedShip);

        UpdateCardText(survivedShip);

        UnsetInRange();

        gameCardList.Remove(destroyedShip);

        DestroyAllChildren(destroyedShip.transform.GetChild(0).gameObject);

        RemoveData();

        DeactivateButtons();
    }
    
    private void UpdateBothShips(GameObject defender, GameObject attacker)
    {
        UpdateCardText(attacker);
        UpdateCardText(defender);

        UnsetInRange();

        RemoveData();

        DeactivateButtons();
    }
    public void CheckWin(GameObject card)
    {
        GameCard gameCard = card.GetComponent<GameCard>();

        if (!gameCard.typeOfShip.Equals(GameManager.TypeOfShips.CargoShip))
        {
            return;
        }

        if (gameCard.deck.Equals(GameManager.TypeOfDeck.RedSuns))
        {
            photonView.RPC("SetWinner", RpcTarget.All, 6); // obiekt WinVuforians
        }
        else
        {
            photonView.RPC("SetWinner", RpcTarget.All, 5); // obiekt WinRedSuns
        }
    }
    [PunRPC]
    public void SetWinner(int numberOfObjectInCanvas)
    {
        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        mainCanvas.transform.GetChild(numberOfObjectInCanvas).gameObject.SetActive(true);
        mainCanvas.transform.GetChild(0).gameObject.SetActive(false);
        mainCanvas.transform.GetChild(2).gameObject.SetActive(false);
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
    public static void UpdateCardText(GameObject card)
    {
        //panel 1
        for (int i = 0; i < card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.childCount; i++)
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = SetText(i, card);
        //panel 2
        for (int i = 0; i < card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.childCount; i++)
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = SetText(i, card);
    }
    private static string SetText(int i, GameObject card) => i switch
    {
        0 => card.GetComponent<GameCard>().cardName.ToString(),
        1 => "HP: " + card.GetComponent<GameCard>().hp.ToString(),
        2 => "Armor: " + card.GetComponent<GameCard>().armor.ToString(),
        3 => "Deflector: " + card.GetComponent<GameCard>().deflectorShield.ToString(),
        4 => "Damage: " + card.GetComponent<GameCard>().damage.ToString(),
        5 => "SetCost: " + card.GetComponent<GameCard>().setCost.ToString(),
        6 => "MovingCost: " + card.GetComponent<GameCard>().movingCost.ToString(),
        _ => throw new ArgumentOutOfRangeException()
    };
    public static GameObject FindNetworkPlayer(dynamic condition, string field)
    {
        GameObject matchPlayer = Array.Find(instance.networkPlayers, player => field switch
        {
            "PlayerID" => player.GetComponent<Player>().playerID.Equals(condition),
            "Deck" => player.GetComponent<Player>().typeOfDeck.Equals(condition),
            "IsMine" => player.GetComponent<PhotonView>().IsMine.Equals(condition),
            "Initiative" => player.GetComponent<Player>().playerID.Equals(condition),
            _ => throw new ArgumentOutOfRangeException()
        });
        return matchPlayer;
    }

    public void RemoveData()
    {
        attackerID = defenderID = 0;
    }

    public void StartHitEffect(GameObject card)
    {
        card.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
    }
    public void StartShootEffect(GameObject card)
    {
        GameObject effectBoxObject = card.transform.GetChild(0).transform.GetChild(2).gameObject;

        switch (card.GetComponent<GameCard>().typeOfWeapon)
        {
            case TypeOfWeapons.Laser:
                effectBoxObject.SetActive(true);

                effectBoxObject.GetComponent<AudioSource>().Play();
                break;
            case TypeOfWeapons.Rackets:

                effectBoxObject.transform.GetChild(0).gameObject.SetActive(true);

                effectBoxObject.GetComponent<AudioSource>().Play();

                effectBoxObject.transform.GetChild(0).gameObject.GetComponent<RocketMovement>().shoot = true;
                break;
            case TypeOfWeapons.Kinetic:
                effectBoxObject.SetActive(true);

                effectBoxObject.GetComponent<AudioSource>().Play();
                break;
            case TypeOfWeapons.None:
                // do nothing
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    #region PunRPC

    [PunRPC]
    private void SyncMakeAttack()
    {

        GameObject defender, attacker;
        attacker = FindGameCardInScene(attackerID, "Attacker");
        defender = FindGameCardInScene(defenderID, "Defender");

        attacker.GetComponent<GameCard>().used = true;

        AttackAction(defender, attacker);
    }

    [PunRPC]
    public void SyncPlayerCredits(int attackerID)
    {
        GameObject gameCard = FindGameCardInScene(attackerID, "ID");

        GameObject player = FindNetworkPlayer(gameCard.GetComponent<GameCard>().deck, "Deck");

        player.GetComponent<Player>().credits -= gameCard.GetComponent<GameCard>().movingCost;

        UpdatePlayerText();
    }

    [PunRPC]
    public void SyncAttacker(int id)
    {
        this.attackerID = id;
    }

    [PunRPC]
    public void SyncDefender(int id)
    {
        this.defenderID = id;
    }

    [PunRPC]
    public void SyncPositionData(int boxPosition, int cardID, int credits)
    {
        GameObject card = FindGameCardInScene(cardID, "ID");
        GameCard gameCard = card.GetComponent<GameCard>();
        GameObject player = FindNetworkPlayer(gameCard.deck, "Deck");

        player.GetComponent<Player>().credits = credits;
        gameCard.cardPosition = boxPosition;

        UpdateCardText(card);

        UpdatePlayerText();

    }

    [PunRPC]
    public void SyncRemoveData()
    {
        SetUnchecked();

        instance.attackerID = instance.defenderID = 0;
    }
    #endregion
}