using System.Collections;
using UnityEngine;

public class ErrorSignTimer : MonoBehaviour
{
    public float timeToDeactivating;
    private void Update()
    {
        GameObject shipButton1 = this.transform.parent.GetChild(2).gameObject;
        GameObject shipButton2 = this.transform.parent.GetChild(3).gameObject;

        if (this.gameObject.activeSelf)
        {
            bool isActiveShipButton1 = shipButton1.activeSelf;
            bool isActiveShipButton2 = shipButton2.activeSelf;
            shipButton1.SetActive(false);
            shipButton2.SetActive(false);
            StartCoroutine(OwnTimer(timeToDeactivating, shipButton1,shipButton2, isActiveShipButton1, isActiveShipButton2));
        }
    }
    private IEnumerator OwnTimer(float timeInSeconds, GameObject shipButton1, GameObject shipButton2,bool isActive1, bool isActive2)
    {
        yield return new WaitForSeconds(timeInSeconds);

        this.gameObject.SetActive(false);
        shipButton1.SetActive(isActive1);
        shipButton2.SetActive(isActive2);
    }
}
