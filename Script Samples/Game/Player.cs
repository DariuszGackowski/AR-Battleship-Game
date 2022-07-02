using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    [Header("Player Informations")]
    public int playerID;
    public string playerName;
    public int credits;
    public GameManager.TypeOfDeck typeOfDeck;

    [Space] public bool isMine;

    private bool setPlayer;

    private void Start()
    {
        this.photonView.RPC("SyncValues", RpcTarget.AllBuffered, playerID, playerName, credits, typeOfDeck);
    }
    private void Update()
    {
        if (!setPlayer)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerList.Add(this.gameObject);
            setPlayer = !setPlayer;
        }
    }
    [PunRPC]
    public void SyncValues(int playerID, string playerName, int credits, GameManager.TypeOfDeck typeOfDeck)
    {
        this.playerID = playerID;
        this.gameObject.name = this.playerName = playerName;
        this.credits = credits;
        this.typeOfDeck = typeOfDeck;
    }
}