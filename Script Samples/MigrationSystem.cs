using UnityEngine;

[System.Obsolete]
public class MigrationSystem : MonoBehaviour/* : NetworkMigrationManager*/
{
    //public override bool FindNewHost(out PeerInfoMessage newHostInfo, out bool youAreNewHost)
    //{
    //    SetHost();
    //    return base.FindNewHost(out newHostInfo, out youAreNewHost);
    //}
    //private void SetHost()
    //{
    //    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerCount = 1;
    //    GameManager.FindNetworkGameObject(true, "Host").GetComponent<PlayerStatistics>().host = true;
    //    GameManager.FindNetworkGameObject(true, "Host").GetComponent<PlayerStatistics>().playerID = 0;
    //    Debug.Log("NewHost");
    //}
}
