using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Obsolete]
public class NetworkSystem : MonoBehaviour/* : NetworkManager*/
{
    //public override void OnStopServer()
    //{
    //    base.OnStopServer();
    //    if (!(GameManager.FindNetworkGameObject(true, "Host") is null))
    //        Debug.Log("ServerStoped + Count: " + GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount);
    //}
    //public override void OnStopClient()
    //{
    //    base.OnStopClient();
    //    if (!(GameManager.FindNetworkGameObject(true, "Host") is null))
    //        Debug.Log("ClientRunAway + Count: " + GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount);
    //}
    //public override void OnStopHost()
    //{
    //    base.OnStopHost();
    //    if (!(GameManager.FindNetworkGameObject(true, "Host") is null))
    //    {
    //        ChangeCount();
    //        ChangeUIData();
    //        UnsetListener();
    //        Debug.Log("HostHaveEnough + Count: " + GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount);
    //    }
    //}
    //public override void OnServerDisconnect(NetworkConnection conn)
    //{
    //    base.OnServerDisconnect(conn);
    //    NetworkServer.DestroyPlayersForConnection(conn);
    //    if (!(GameManager.FindNetworkGameObject(true, "Host") is null))
    //    {
    //        ChangeCount();
    //        ChangeUIData();
    //        UnsetListener();
    //        Debug.Log("ServerDisconnect + Count: " + GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount);
    //    }
    //}
    //public override void OnClientDisconnect(NetworkConnection conn)
    //{
    //    base.OnClientDisconnect(conn);
    //    if (!(GameManager.FindNetworkGameObject(true, "Host") is null))
    //        Debug.Log("ClientDisconnect + Count: " + GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount);
    //}
    //private void ChangeCount()
    //{
    //    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount -= 1;
    //}
    //private void ChangeUIData()
    //{
    //    GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round = 0;
    //    GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative = 1;
    //}
    //private void UnsetListener() 
    //{
    //    GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).transform.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
    //}
    //private void SetHost()
    //{
    //    (GameManager.FindNetworkGameObject(true, "Host") ?? GameManager.FindNetworkGameObject(false, "Host")).GetComponent<PlayerStatistics>().host = false ? true : true;
    //}
}
