using System;
using TMPro;
using UnityEngine;

[System.Obsolete]
public class GameUserInterface : MonoBehaviour/*: NetworkBehaviour*/
{
    //[SyncVar(hook = "SetDataUI")]
    public int round;
    //[SyncVar(hook = "CheckPlayerInitiative")]
    public int playerInitiative;
    public GameObject playerInfo1, playerInfo2, gameInfo;
    //public void SetDataUI(int round)
    //{
    //    if (round.Equals(1))
    //    {
    //        GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).gameObject.SetActive(true);
    //        playerInfo1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = FindUIPlayerData(isServer, "Left", "Name");
    //        playerInfo1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Credits: " + FindUIPlayerData(isServer, "Left", "Credits");
    //        playerInfo2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = FindUIPlayerData(isServer, "Right", "Name");
    //        playerInfo2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Credits: " + FindUIPlayerData(isServer, "Right", "Credits");
    //        gameInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Round: " + round.ToString();
    //        gameInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Initiative: " + SetPlayerInitiative(playerInitiative).ToString();
    //    }
    //    else
    //    {
    //        GameManager.FindNetworkGameObject(true, "Host").GetComponent<PlayerStatistics>().credits += GameManager.startCredits[GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round];
    //        GameManager.FindNetworkGameObject(false, "Host").GetComponent<PlayerStatistics>().credits += GameManager.startCredits[GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round];
    //        playerInfo1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Credits: " + FindUIPlayerData(isServer, "Left", "Credits");
    //        playerInfo2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Credits: " + FindUIPlayerData(isServer, "Right", "Credits");
    //    }
    //}
    //public void CheckPlayerInitiative(int playerInitiative)
    //{
    //    SetPlayerInitiativeData(playerInitiative);
    //    Debug.Log(this.playerInitiative.ToString() + "|||||" + playerInitiative.ToString());
    //}
    //private void SetPlayerInitiativeData(int playerInitiative)
    //{
    //    gameInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Initiative: " + SetPlayerInitiative(playerInitiative).ToString();
    //    this.playerInitiative = playerInitiative; ///od hosta do clienta
    //    Debug.Log(this.playerInitiative.ToString() + "||Robie..czy nie robie?|||" + playerInitiative.ToString());
    //}
    private string SetPlayerInitiative(int playerInitiative) => playerInitiative switch
    {
        1 => GameManager.FindNetworkGameObject(true, "Host").name,
        2 => GameManager.FindNetworkGameObject(false, "Host").name,
        _ => throw new ArgumentOutOfRangeException()
    };
    private dynamic FindUIPlayerData(bool isServer, string side, string field) => field switch
    {
        "Name" => SetUIPlayerName(isServer, side),
        "Credits" => SetUIPlayerCredits(isServer, side),
        _ => throw new ArgumentOutOfRangeException()
    };
    private string SetUIPlayerCredits(bool isServer, string side) => isServer switch
    {
        true => GameManager.FindNetworkGameObject(SetServerBool(isServer, side), "Host").GetComponent<PlayerStatistics>().credits.ToString(),
        false => GameManager.FindNetworkGameObject(SetServerBool(isServer, side), "Host").GetComponent<PlayerStatistics>().credits.ToString()
    };
    private string SetUIPlayerName(bool isServer, string side) => isServer switch
    {
        true => GameManager.FindNetworkGameObject(SetServerBool(isServer, side), "Host").GetComponent<PlayerStatistics>().playerName,
        false => GameManager.FindNetworkGameObject(SetServerBool(isServer, side), "Host").GetComponent<PlayerStatistics>().playerName
    };
    private bool SetServerBool(bool isServer, string side) => side switch
    {
        "Right" => !isServer,
        _ => isServer
    };
}
