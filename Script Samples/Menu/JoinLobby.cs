using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField joinInput;

    public void Cancel() 
    {
        StartCoroutine(WaitSecondsCancel(0.5f));
    }

    public void JoinRoom()
    {
        StartCoroutine(WaitSecondsJoinRoom(0.5f));
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("Game");
    }

    private IEnumerator WaitSecondsJoinRoom(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        PhotonNetwork.JoinRoom(joinInput.text.ToString());
    }
    private IEnumerator WaitSecondsCancel(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        PhotonNetwork.Disconnect();

        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        SceneManager.LoadScene("Deck");
    }
}
