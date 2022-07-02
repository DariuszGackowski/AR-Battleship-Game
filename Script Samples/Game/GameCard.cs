using UnityEngine;
public class GameCard : MonoBehaviour
{
    [Header("Card Informations")]
    [Space] public bool inRange;
    public bool used;

    [Header("Card Types")]
    [Space] public int id;
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

    private void Awake()
    {
        cardName = gameObject.name.ToString();

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameCardList.Add(this.gameObject);

        GameManager.UpdateCardText(this.gameObject);
    }
}
