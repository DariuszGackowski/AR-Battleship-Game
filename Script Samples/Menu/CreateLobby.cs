using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.SceneManagement;

public class CreateLobby : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    [SerializeField] private TMP_InputField createInput;

    public void Cancel()
    {
        StartCoroutine(WaitSecondsCancel(0.5f));
    }
    public void CreateRoom()
    {
        StartCoroutine(WaitSecondsCreateRoom(0.5f));
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("Game");
    }



    private IEnumerator WaitSecondsCreateRoom(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createInput.text.ToString(), roomOptions);
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
