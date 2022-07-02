using System.Collections;
using UnityEngine;

public class ErrorSignTimer : MonoBehaviour
{
    public float timeToDeactivating;
    private void Update()
    {
        GameObject shipButton = this.transform.parent.GetChild(1).gameObject;

        if (this.gameObject.activeSelf)
        {
            shipButton.SetActive(false);
            StartCoroutine(OwnTimer(timeToDeactivating, shipButton));
        }
    }
    private IEnumerator OwnTimer(float timeInSeconds, GameObject shipButton)
    {
        yield return new WaitForSeconds(timeInSeconds);

        this.gameObject.SetActive(false);
        shipButton.SetActive(true);
    }
}
