using System;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class PlayerDataBase : PlayerData
{
    //public SyncListPlayerStruct syncPlayersList = new SyncListPlayerStruct();
    public static List<PlayerData.PlayerStruct> unSyncPlayersList = new List<PlayerData.PlayerStruct>();
    public static bool NotUpdated = true;

    //public void SetSyncList()
    //{ 
    //    if (!syncPlayersList.Count.Equals(0))
    //        syncPlayersList.Clear();
    //    foreach (PlayerStruct player in unSyncPlayersList)
    //        syncPlayersList.Add(player);
    //    foreach (PlayerStruct player in unSyncPlayersList)
    //    {
    //        Debug.Log(player.PlayerName.ToString() + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    //    }
    //    foreach (PlayerStruct player in syncPlayersList)
    //    {
    //        Debug.Log(player.PlayerName.ToString() + "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
    //    }
    //}
    //public PlayerStruct FindPlayer(int playerID) 
    //{
    //    PlayerStruct returnPlayer = new PlayerStruct(0,null,0,GameCard.TypeOfDeck.BlackSuns);
    //    foreach (PlayerStruct player in syncPlayersList)
    //        if (player.PlayerID.Equals(playerID))
    //            returnPlayer = player;
    //    return returnPlayer;
    //}
    public void UpdateSyncPlayersList(dynamic data, string field, PlayerStruct player)
    {
        switch (field)
        {
            case "PlayerName":
                player.PlayerName = data;
                break;
            case "Credits":
                player.Credits = data;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}