using UnityEngine;

public class CardsDetector : MonoBehaviour
{
    [SerializeField] private int boxPosition;

    private GameManager gameManager;
    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("SpaceShip"))
        {
            return;
        }

        GameObject card = other.transform.parent.transform.parent.gameObject;
        GameCard cardCard = card.GetComponent<GameCard>();

        if (cardCard.cardPosition.Equals(boxPosition)) /// jêsli pozycja statku ta sama co boxu
        {
            return;
        }

        if (!GameManager.FindNetworkPlayer(cardCard.deck, "Deck").GetComponent<Player>().playerID.Equals(gameManager.initiative)) // jeœli karta jest innego gracza
        {
            card.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true); // uaktywnienie znaku zakazu
            return;
        }

        if (cardCard.cardPosition.Equals(0))
        {
            gameManager.SetCardPosition(card, boxPosition, "NewCard");
        }
        else
        {
            gameManager.SetCardPosition(card, boxPosition, "ChangePosition");
        }
    }
}