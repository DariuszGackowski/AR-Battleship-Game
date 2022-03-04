using System;
using UnityEngine;

[System.Obsolete]
public class PlayerStatistics : PlayerData
{
    //[SyncVar]
    public int playerID;
    //[SyncVar]
    public string playerName;
    //[SyncVar]
    public int credits;
    //[SyncVar]
    public GameCard.TypeOfDeck typeOfDeck;
    //[SyncVar]
    public bool host;
    private void Update()
    {
        if (!this.gameObject.name.Equals(playerName))
            this.gameObject.name = playerName;
    }
    public void PlayerUpdate()
    {
        PlayerDataBase.unSyncPlayersList.Add(new PlayerStruct(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount, playerName.ToString(), credits, typeOfDeck));
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataBase>().SetSyncList();
    }
    //public override void OnStartLocalPlayer()
    //{
    //    base.OnStartLocalPlayer();
    //    if (isLocalPlayer)
    //    {
    //        playerName = this.gameObject.name = PlayerPrefs.GetString("Name");
    //        typeOfDeck = (GameCard.TypeOfDeck)Enum.Parse(typeof(GameCard.TypeOfDeck), PlayerPrefs.GetString("TypeOfDeck"));
    //    }
    //    if (isServer)
    //        host = true;
    //    CmdSendDataToServer(PlayerPrefs.GetString("Name"), (GameCard.TypeOfDeck)Enum.Parse(typeof(GameCard.TypeOfDeck), PlayerPrefs.GetString("TypeOfDeck")));
    //    CmdSetupPlayer();
    //    //PlayerUpdate();
    //}
    public void SetHostData()
    {
        PlayerStatistics host = GameManager.FindNetworkGameObject(PlayerPrefs.GetString("Name"), "Name").GetComponent<PlayerStatistics>();
        PlayerStatistics noHost = GameManager.FindNetworkGameObject(PlayerPrefs.GetString("Name"), "NoName").GetComponent<PlayerStatistics>();
        host.SetPlayerData(host.playerID, host.playerName, host.credits, host.typeOfDeck);
        noHost.SetPlayerData(noHost.playerID, noHost.playerName, noHost.credits, noHost.typeOfDeck);
    }
    private void SetPlayerData(int playerID, string name, int credits, GameCard.TypeOfDeck typeOfDeck)
    {
        this.playerID = playerID;
        this.playerName = name;
        this.credits = credits;
        this.typeOfDeck = typeOfDeck;
    }
    //[Command]
    //private void CmdSendDataToServer(string name, GameCard.TypeOfDeck typeOfDeck)
    //{
    //    RpcSetPlayerName(name);
    //    RpcSetTypeOfDeck(typeOfDeck);
    //}
    //[Command]
    //private void CmdSetupPlayer()
    //{
    //    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().AddPlayer(this);
    //    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount++;
    //}
    //[ClientRpc]
    //private void RpcSetPlayerName(string name)
    //{
    //    this.playerName = this.gameObject.name = name;
    //    if (!(GameManager.FindNetworkGameObject(false, "Host") is null) && GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round.Equals(0))
    //        GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round += 1;
    //}
    //[ClientRpc]
    //private void RpcSetTypeOfDeck(GameCard.TypeOfDeck typeOfDeck)
    //{
    //    this.typeOfDeck = typeOfDeck;
    //}
}