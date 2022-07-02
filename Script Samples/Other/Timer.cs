using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeToDeactivating;
    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(OwnTimer(timeToDeactivating));
        }
    }
    private IEnumerator OwnTimer(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        this.gameObject.SetActive(false);
    }
}