using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class ShipButton : MonoBehaviourPun
{
    [NonSerialized] public bool isSelected;

    private GameManager gameManager;
    private EventSystem eventSystem;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
    }

    public void ButtonClick()
    {
        GameObject card = gameObject.transform.parent.transform.parent.transform.parent.gameObject;
        GameCard gameCard = card.GetComponent<GameCard>();

        if (gameCard.used) // zwracam kiedy obiekt jest u¿yty
        {
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);// znak zakazu aktywuje
            return;
        }

        if (gameManager.attackerID.Equals(0) && gameCard.typeOfShip.Equals(GameManager.TypeOfShips.CargoShip)) // jeœli wybieramy frachtowiec
        {
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);

            GameManager.DeactivateButtons();

            return;
        }

        if (gameManager.attackerID.Equals(0) && !gameCard.deck.Equals(gameManager.typeOfDeck)) // blokuje u¿ycie nie swojej karty
        {
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);// znak zakazu aktywuje
            return;
        }

        if (gameManager.attackerID.Equals(0) && !GameManager.CheckCredits(card)) //CheckCredits sprawdza czy staæ gracza na ruch swoj¹ kart¹ ||| blokuje u¿ycie karty gracza, który jest bez pieniêdzy
        {
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);// znak zakazu aktywuje
            return;
        }
        if (!gameManager.attackerID.Equals(0) && !gameCard.inRange || !gameManager.attackerID.Equals(0) && !gameCard.id.Equals(gameManager.attackerID) && gameCard.deck.Equals(GameManager.FindGameCardInScene(gameManager.attackerID, "ID").GetComponent<GameCard>().deck)) /// blokuje jeœli obroñca jest tej samej tali co atakuj¹cy)
        {
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true); // znak zakazu aktywuje

            GameManager.DeactivateButtons();

            photonView.RPC("SyncRemoveData", RpcTarget.All);

            return;
        }
        if (isSelected.Equals(false))
        {
            if (gameManager.attackerID.Equals(0))
            {
                eventSystem.SetSelectedGameObject(this.gameObject);
            }
            gameManager.ActivateSceneObject(card);

            isSelected = true;
        }
        else
        {
            eventSystem.SetSelectedGameObject(null);

            gameManager.ActivateSceneObject(card);

            isSelected = false;
        }
    }
    [PunRPC]
    public void SyncRemoveData()
    {
        GameManager.SetUnchecked();

        gameManager.attackerID = gameManager.defenderID = 0;
    }
    public static void ClearColor(GameObject button)
    {
        EventSystem eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();

        eventSystem.SetSelectedGameObject(null);

        ShipButton shipButton = button.GetComponent<ShipButton>();

        shipButton.isSelected = false;

    }
}
