using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void CreateGame()
    {
        StartCoroutine(WaitSecondsDeck(0.5f));
        ChoiceContainer.decision = 1;
    }
    public void AddToGame() 
    {
        StartCoroutine(WaitSecondsDeck(0.5f));
        ChoiceContainer.decision = 2;
    }
    public void Exit()
    {
        StartCoroutine(WaitSecondsQuit(0.5f));
    }
    private IEnumerator WaitSecondsDeck(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        SceneManager.LoadScene("Deck");
    }
    private IEnumerator WaitSecondsQuit(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        Application.Quit();
    }
}
