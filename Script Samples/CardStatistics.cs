using UnityEngine;

[System.Obsolete]
public class CardStatistics : MonoBehaviour
{
    [Header("Card Settings")]
    public bool inRange;
    public int id;
    public GameCard.TypeOfDeck deck;
    public string cardName;
    public GameCard.TypeOfShips typeOfShip;
    public GameCard.TypeOfWeapons typeOfWeapon;
    public int setCost;
    public int movingCost;
    public int hp;
    public int armor;
    public int deflectorShield;
    public int damage;
    public int cardPosition;
    private void Awake()
    {
        GameCardsDataBase.GameCardList.Add(new GameCard(id, deck, cardName, typeOfShip, typeOfWeapon, setCost, movingCost, hp, armor, deflectorShield, damage, cardPosition));
        GameManager.UpdateCardText(this.gameObject);
    }
}