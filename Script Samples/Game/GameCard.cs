using UnityEngine;
public class GameCard : MonoBehaviour
{
    [Header("Card Informations")]
    public bool inRange;
    public bool used;

    [Header("Card Types")]
    public int id;
    public GameManager.TypeOfDeck deck;
    public string cardName;
    public GameManager.TypeOfShips typeOfShip;
    public GameManager.TypeOfWeapons typeOfWeapon;

    [Header("Card Points")]
    public int setCost;
    public int movingCost;
    public int hp;
    public int armor;
    public int deflectorShield;
    public int damage;
    public int cardPosition;

    private void Start()
    {
        cardName = cardName.ToString();

        GameManager.instance.gameCardList.Add(this.gameObject);

        GameManager.UpdateCardText(this.gameObject);
    }
}
