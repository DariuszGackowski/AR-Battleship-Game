using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetDeck : MonoBehaviour
{
    public void BlackSuns()
    {
        PlayerPrefs.SetString("TypeOfDeck", GameManager.TypeOfDeck.BlackSuns.ToString());

        StartCoroutine(WaitSecondsLoading(0.5f));
    }
    public void Vuforians()
    {
        PlayerPrefs.SetString("TypeOfDeck", GameManager.TypeOfDeck.Vuforians.ToString());

        StartCoroutine(WaitSecondsLoading(0.5f));
    }
    public void Cancel()
    {
        Destroy(GameObject.FindGameObjectWithTag("ChoiceContainer"));

        StartCoroutine(WaitSecondsCancel(0.5f));
    }
    private IEnumerator WaitSecondsLoading(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        SceneManager.LoadScene("Loading");
    }
    private IEnumerator WaitSecondsCancel(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        SceneManager.LoadScene("MainMenu");
    }
}
