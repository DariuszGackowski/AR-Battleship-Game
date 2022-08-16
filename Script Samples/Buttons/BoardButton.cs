using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardButton : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    [NonSerialized] public bool isSelected;

    public void ButtonClick()
    {
        if (isSelected.Equals(false))
        {
            ButtonSetActive(this.gameObject, true);

            eventSystem.SetSelectedGameObject(this.gameObject);

            isSelected = true;
        }
        else
        {
            ButtonSetActive(this.gameObject, false);

            eventSystem.SetSelectedGameObject(null);

            isSelected = false;
        }
    }
    private void ButtonSetActive(GameObject button, bool active)
    {
        GameObject box = button.transform.parent.transform.parent.transform.GetChild(2).gameObject;

        for (int i = 0; i < box.transform.childCount; i++)
        {
            box.transform.GetChild(i).gameObject.SetActive(active);
        }
    }
}

