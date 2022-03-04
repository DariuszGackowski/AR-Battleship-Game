using System.Collections;
using UnityEngine;
public class SignTimer : MonoBehaviour
{
    private void Update()
    {
        if (this.gameObject.activeSelf)
            StartCoroutine(Timer(1));
    }
    private IEnumerator Timer(int timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        this.gameObject.SetActive(false);
    }
}