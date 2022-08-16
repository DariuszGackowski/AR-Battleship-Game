using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class GameUserInterface : MonoBehaviourPun
{
    [SerializeField] private GameObject playerInfo1, playerInfo2, creditsInfo1, creditsInfo2, waitingInfo, next, groupInfo1, groupInfo2, groupInfo3,upPanel;

    [SerializeField] public GameObject roundInfo, initiativeInfo;

    [NonSerialized] public bool startUI;

    [Space][SerializeField] private bool setUI;

    [NonSerialized] public GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void Update()
    {
        if (!setUI && gameManager.playersAreSet)
        {
            SetDataUI();
        }
    }
    private void SetDataUI()
    {
        playerInfo1.GetComponent<TextMeshProUGUI>().text = SetUIPlayerName("Left");
        playerInfo2.GetComponent<TextMeshProUGUI>().text = SetUIPlayerName("Right");

        UpdateCreditsText();

        roundInfo.GetComponent<TextMeshProUGUI>().text = "Round: " + gameManager.round.ToString();

        initiativeInfo.GetComponent<TextMeshProUGUI>().text = "Initiative: " + SetPlayerInitiative(gameManager.initiative).ToString();

        SetGameObjectActive();

        setUI = true;
    }
    private void SetGameObjectActive()
    {
        waitingInfo.SetActive(false);

        upPanel.SetActive(true);

        next.SetActive(true);
    }
    public void UpdateCreditsText()
    {
        creditsInfo1.GetComponent<TextMeshProUGUI>().text = "Credits: " + SetUIPlayerCredits("Left");
        creditsInfo2.GetComponent<TextMeshProUGUI>().text = "Credits: " + SetUIPlayerCredits("Right");
    }
    public string SetPlayerInitiative(int playerInitiative) => playerInitiative switch
    {
        1 => GameManager.FindNetworkPlayer(1, "Initiative").name,
        2 => GameManager.FindNetworkPlayer(2, "Initiative").name,
        _ => throw new ArgumentOutOfRangeException()
    };
    public static string SetUIPlayerCredits(string side) => side switch
    {
        "Left" => GameManager.FindNetworkPlayer(true, "IsMine").GetComponent<Player>().credits.ToString(),
        "Right" => GameManager.FindNetworkPlayer(false, "IsMine").GetComponent<Player>().credits.ToString(),
        _ => throw new ArgumentOutOfRangeException()
    };
    private string SetUIPlayerName(string side) => side switch
    {
        "Left" => GameManager.FindNetworkPlayer(true, "IsMine").name.ToString(),
        "Right" => GameManager.FindNetworkPlayer(false, "IsMine").name.ToString(),
        _ => throw new ArgumentOutOfRangeException()
    };
}
