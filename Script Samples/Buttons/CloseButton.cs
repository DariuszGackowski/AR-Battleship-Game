using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseButton : MonoBehaviourPun
{
    public void ExitRoom() 
    { 
        this.photonView.RPC("SetWin", RpcTarget.Others);

        StartCoroutine(WaitSecondsClose(0.5f));
    }

    [PunRPC]
    public void SetWin() 
    {
        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");

        mainCanvas.transform.GetChild(0).gameObject.SetActive(false); //  UpPanel

        mainCanvas.transform.GetChild(2).gameObject.SetActive(false); // Next button

        if (!mainCanvas.transform.GetChild(5).gameObject.activeSelf && !mainCanvas.transform.GetChild(6).gameObject.activeSelf) // sprawdzenie czy panele zwyciestwa nie s� w��czone
        {
            Debug.Log(mainCanvas.transform.GetChild(5).gameObject.activeSelf + "/// " + mainCanvas.transform.GetChild(6).gameObject.activeSelf);
            mainCanvas.transform.GetChild(4).gameObject.SetActive(true); // closePanel
        }
    }

    private IEnumerator WaitSecondsClose(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        PhotonNetwork.LeaveRoom();

        PhotonNetwork.Disconnect();

        while (PhotonNetwork.InRoom && PhotonNetwork.IsConnected) // dop�ki jest w menu zwracaj nulla, po wyj�ciu dopiero wczytaj scene
        {
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}
