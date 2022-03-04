using System;
using UnityEngine;

[System.Serializable]
[System.Obsolete]
public class GameCard : MonoBehaviour/*: NetworkBehaviour*/
{
    private int _id;
    public int ID
    {
        get => _id;
        set => _id = value;
    }
    public enum TypeOfDeck { Vuforians, BlackSuns};
    private TypeOfDeck _deck;
    public TypeOfDeck Deck
    {
        get => _deck;
        set => _deck = value;
    }
    private string _cardName;
    public string CardName
    {
        get => _cardName;
        set => _cardName = value;
    }
    public enum TypeOfShips { Corvette, Destroyer, Cruiser };
    private TypeOfShips _TypeOfShip;
    public TypeOfShips TypeOfShip
    {
        get => _TypeOfShip;
        set => _TypeOfShip = value;
    }
    public enum TypeOfWeapons { Laser, Rackets, Kinetic };
    private TypeOfWeapons _TypeOfWeapon;
    public TypeOfWeapons TypeOfWeapon
    {
        get => _TypeOfWeapon;
        set => _TypeOfWeapon = value;
    }
    private int _setCost;
    public int SetCost
    {
        get => _setCost;
        set => _setCost = value;
    }
    private int _movingCost;
    public int MovingCost
    {
        get => _movingCost;
        set => _movingCost = value;
    }
    private int _hp;
    public int HP
    {
        get => _hp;
        set => _hp = value;
    }
    private int _armor;
    public int Armor
    {
        get => _armor;
        set => _armor = value;
    }
    private int _deflectorShield;
    public int DeflectorShield
    {
        get => _deflectorShield;
        set => _deflectorShield = value;
    }
    private int _damage;
    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }
    private int _cardPosition;
    public int CardPosition
    {
        get => _cardPosition;
        set => _cardPosition = value;
    }
    public static int[][] CardPositionData = new int[12][]
    {
        new int[] {},
        new int[] {3},
        new int[] {3,5},
        new int[] {1,2,4,6},
        new int[] {3,7},
        new int[] {2,8},
        new int[] {3,9},
        new int[] {4,10},
        new int[] {5,9},
        new int[] {6,8,10,11},
        new int[] {7,9},
        new int[] {9}
    };
    public static int[][] RacketsAvailabelPositions = new int[12][]
    {
        new int[] {},
        new int[] {2,3,4,5,6,7},
        new int[] {1,3,4,5,6,8},
        new int[] {1,2,4,5,6,7,9},
        new int[] {1,2,3,6,7,10},
        new int[] {2,3,8,9},
        new int[] {1,2,3,4,8,9,10,11},
        new int[] {3,4,9,10},
        new int[] {2,5,6,9,10,11},
        new int[] {3,5,6,7,8,10,11},
        new int[] {4,6,7,8,9,11},
        new int[] {5,6,7,8,9,10}
    };
    public int[][,] ShipSegments = new int[3][,]
    {
        new int[,] {{ 99 },{ 99 }},
        new int[,] {{ 99, 99 },{ 99, 99 }},
        new int[,] {{ 99, 99, 99 , 99 },{ 99, 99, 99, 99 }}
    };
    public GameCard(int id, TypeOfDeck deck, string cardName, TypeOfShips typeOfShip, TypeOfWeapons typeOfWeapon, int setCost, int movingCost, int hp, int armor, int deflectorShield, int damage, int cardPosition)
    {
        this.ID = id;
        this.Deck = deck;
        this.CardName = cardName;
        this.TypeOfShip = typeOfShip;
        this.TypeOfWeapon = typeOfWeapon;
        this.SetCost = setCost;
        this.MovingCost = movingCost;
        this.HP = hp;
        this.Armor = armor;
        this.DeflectorShield = deflectorShield;
        this.Damage = damage;
        this.CardPosition = cardPosition;
        UpdateCardShipSegments(typeOfShip, hp, armor, this.ShipSegments);
    }
    public static void UpdateCardShipSegments(TypeOfShips typeOfShip, int hp, int armor, int[][,] shipSegments)
    {
        switch (typeOfShip)
        {
            case TypeOfShips.Corvette:
                int[,] oneSegment = new[,] { { hp }, { armor } };
                shipSegments[0] = oneSegment;
                break;
            case TypeOfShips.Destroyer:
                if (shipSegments[1][0, 0].Equals(0))
                {
                    shipSegments[1][0, 1] = hp;
                    shipSegments[1][1, 1] = armor;
                }
                else
                {
                    int[,] twoSegments = new[,] { { hp / 2, hp - hp / 2 }, { armor / 2, armor - armor / 2 } };
                    shipSegments[1] = twoSegments;
                }
                break;
            case TypeOfShips.Cruiser:
                if (shipSegments[2][0, 1].Equals(0) && shipSegments[2][0, 3].Equals(0) && shipSegments[2][0, 0].Equals(0))
                {
                    shipSegments[2][0, 2] = hp;
                    shipSegments[2][1, 2] = armor;
                }
                else if (shipSegments[2][0, 3].Equals(0) && shipSegments[2][0, 0].Equals(0))
                {
                    int[,] twoSegments = new[,] { { hp / 2, hp - hp / 2 }, { armor / 2, armor - armor / 2 } };
                    for (int i = 0; i < GameManager.ControledLength(shipSegments[2].Length) - 2; i++)
                        for (int j = 0; j < GameManager.ControledLength(shipSegments[2].Length) - 2; j++)
                            shipSegments[2][i, j + 1] = twoSegments[i, j];
                }
                else if (shipSegments[2][0, 0].Equals(0))
                {
                    int[,] threeSegments = new[,] { { hp / 3, hp - (hp / 3 + (hp - hp / 3) / 2), (hp - hp / 3) / 2 }, { armor / 3, armor - (armor / 3 + (armor - armor / 3) / 2), (armor - armor / 3) / 2 } };
                    for (int i = 0; i < GameManager.ControledLength(shipSegments[2].Length) - 1; i++)
                        for (int j = 3; j < GameManager.ControledLength(shipSegments[2].Length) - 1; j++)
                        {
                            shipSegments[2][i, j] = threeSegments[i, j - 1];
                            j = SetI(j);
                        }
                }
                else
                {
                    int[,] fourSegments = new[,] { { hp / 4, (hp - ((hp / 4) + ((hp - hp / 4) / 3))) / 2, hp - ((hp / 4) + ((hp - hp / 4) / 3) + (hp - ((hp / 4) + ((hp - hp / 4) / 3))) / 2), (hp - hp / 4) / 3 }, { armor / 4, (armor - ((armor / 4) + ((armor - armor / 4) / 3))) / 2, armor - ((armor / 4) + ((armor - armor / 4) / 3) + (armor - ((armor / 4) + ((armor - armor / 4) / 3))) / 2), (armor - armor / 4) / 3 } };
                    shipSegments[2] = fourSegments;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public static int SetI(int j) => j switch
    {
        3 => j -= 3,
        _ => j
    };
}