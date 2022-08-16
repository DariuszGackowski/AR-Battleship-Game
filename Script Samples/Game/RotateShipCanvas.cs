using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShipCanvas : MonoBehaviour
{
    [SerializeField] private GameObject pannelToDisable;
    [SerializeField] private GameObject pannelToAnable;
    [SerializeField] private GameObject secondRotateButton;
    [SerializeField] private GameObject shipButtonToDisable;
    [SerializeField] private GameObject shipButtonToEnable;

    public void RotateCanvas()
    {
        pannelToDisable.SetActive(false);
        pannelToAnable.SetActive(true);

        shipButtonToDisable.SetActive(false);
        shipButtonToEnable.SetActive(true);

        secondRotateButton.SetActive(true);
        this.gameObject.SetActive(false); // button
    }
}
