using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextButton : MonoBehaviour
{
    private GameManager gameManager;
    private GameUserInterface gameUserInterface;
    private PhotonView photonView;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameUserInterface = GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>();

        photonView = GetComponent<PhotonView>();
    }
    public void Next()
    {
        if (!gameManager.initiative.Equals(gameManager.playerInitiativeNumber))
        {
            return;
        }

        photonView.RPC("SyncDeactivateButtons", RpcTarget.All);
        photonView.RPC("SyncRemoveData", RpcTarget.All);

        if (gameManager.initiative.Equals(1))
        {
            photonView.RPC("UpdateInitiative", RpcTarget.All, gameManager.initiative + 1);
        }
        else if (gameManager.initiative.Equals(2))
        {
            photonView.RPC("UpdateRound", RpcTarget.All, gameManager.round + 1);
            photonView.RPC("UpdateInitiative", RpcTarget.All, gameManager.initiative - 1);
        }
    }

    public void UpdatePlayersCredits(int round)
    {
        foreach (GameObject player in gameManager.playerList)
        {
            Player playerScript = player.GetComponent<Player>();
            playerScript.credits += GameManager.startCredits[round];
        }
    }

    private void SetUncheckedCardBoard() 
    {
        GameObject cardBoard = GameObject.FindGameObjectWithTag("CardBoard");
        GameObject boardButton = cardBoard.transform.GetChild(1).transform.GetChild(0).gameObject;
        GameObject boxBoard = cardBoard.transform.GetChild(2).gameObject;

        boardButton.GetComponent<BoardButton>().isSelected = false;

        EventSystem eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(null);

        for (int i = 0; i < boxBoard.transform.childCount; i++)
        {
            boxBoard.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void UpdateInitiative(int initiative)
    {
        gameManager.initiative = initiative;
        gameUserInterface.initiativeInfo.GetComponent<TextMeshProUGUI>().text = "Initiative: " + gameUserInterface.SetPlayerInitiative(initiative).ToString();
    }

    [PunRPC]
    public void UpdateRound(int round)
    {
        gameManager.round = round;

        gameUserInterface.roundInfo.GetComponent<TextMeshProUGUI>().text = "Round: " + round.ToString();

        UpdatePlayersCredits(round);
        gameUserInterface.UpdateCreditsText(); ;
    }

    [PunRPC]
    public void SyncDeactivateButtons()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");

        foreach (GameObject card in cards)
        {
            GameObject button = card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).gameObject;

            ShipButton.ClearColor(button);//czyszczenie kolorów buttonów
        }

        SetUncheckedCardBoard();
    }

    [PunRPC]
    public void SyncRemoveData()
    {
        GameManager.SetUnchecked();

        gameManager.attackerID = gameManager.defenderID = 0;
    }
}
