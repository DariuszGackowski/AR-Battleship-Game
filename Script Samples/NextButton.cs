using UnityEngine;
using UnityEngine.UI;

[System.Obsolete]
public class NextButton : MonoBehaviour/* : NetworkBehaviour*/
{
    //private void Start()
    //{
    //    if (isServer)
    //        GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(NextRoundHost);
    //    else
    //        GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(NextRoundNoHost);///nohost
    //}
    //public void NextRoundHost()/////dla hosta
    //{
    //    if (IsMyMove(GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative))
    //        if ((GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative % 2).Equals(0))
    //        {
    //            GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative = 1;
    //            GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round++;
    //            Debug.Log("Ne hej h");
    //        }
    //        else
    //        {
    //            GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative = 2;
    //            Debug.Log("Hejka h");
    //        }
    //}
    //public void NextRoundNoHost()
    //{
    //    if (IsMyMove(GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative))
    //        if ((GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative % 2).Equals(0))
    //        {
    //            CmdSendData(1);
    //            Debug.Log("Ne hej nh");
    //        }
    //        else
    //        {
    //            CmdSendData(2);
    //            Debug.Log("Hejka nh");
    //        }
    //}
    //private bool IsMyMove(int playerInitiative)
    //{
    //    bool trueOrFalse = false;
    //    if (playerInitiative.Equals(1) && isServer)
    //        trueOrFalse = true;
    //    else if (playerInitiative.Equals(2) && !isServer)
    //        trueOrFalse = true;
    //    Debug.Log("||||||||" + trueOrFalse.ToString());
    //    return trueOrFalse;
    //}
    //[Command]
    //private void CmdSendData(int palyerInititive)
    //{
    //    if (palyerInititive.Equals(1))
    //    {
    //        GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative = palyerInititive;
    //        GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round++;
    //        Debug.Log("Dzia³a");
    //    }
    //    else
    //    {
    //        GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative = palyerInititive;
    //        Debug.Log("Mo¿e dzia³a");
    //    }
    //}
    private void RpcSendInitiativeData(int palyerInititive)
    {
        GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().playerInitiative = palyerInititive;
    }
    private void RpcSendRoundData()
    {
        GameObject.FindGameObjectWithTag("UI").GetComponent<GameUserInterface>().round++;
    }
}
