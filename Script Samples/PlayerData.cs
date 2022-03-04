using UnityEngine;

[System.Obsolete]
public class PlayerData : MonoBehaviour/*: NetworkBehaviour*/
{
    public struct PlayerStruct
    {
        public int PlayerID;
        public string PlayerName;
        public int Credits;
        public GameCard.TypeOfDeck TypeOfDeck;
        public PlayerStruct(int playerID, string player, int credits, GameCard.TypeOfDeck typeOfDeck)
        {
            this.PlayerID = playerID;
            this.PlayerName = player;
            this.Credits = credits;
            this.TypeOfDeck = typeOfDeck;
        }
    }
    //public class SyncListPlayerStruct : SyncListStruct<PlayerStruct> { }
}