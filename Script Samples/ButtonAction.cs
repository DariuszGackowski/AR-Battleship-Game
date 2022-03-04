using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete]
public class ButtonAction : MonoBehaviour
{
    [Header("Button Colors")]
    public Color normalColor;
    public Color selectedColor;
    private void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            ButtonClick(Input.GetTouch(0).position);
        else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
            ButtonClick(Input.GetTouch(0).position);
        ////////Do usuniecia, po skoñczeniu projektu//////////////////////////////////////////////////////////////////////////
        if (Input.GetMouseButtonDown(0))
            ButtonClick(Input.mousePosition);
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
    private void ButtonClick(Vector3 inputPosition)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(inputPosition), out RaycastHit Hit) && Hit.transform.CompareTag("Button"))
        {
            Dictionary<int, GameObject> mainDictionary = new Dictionary<int, GameObject>();
            mainDictionary = IsActive(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons, mainDictionary);
            mainDictionary.TryGetValue(1, out GameObject getButton);
            if (mainDictionary.Count.Equals(0))
            {
                int listLength = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons.Count;
                for (int i = 0; i < listLength; i++)
                {
                    if (GameManager.actionDataBase.ActionID.Equals(0) && !Hit.transform.GetComponent<ButtonID>().Equals(null) && !GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons[i].name.Equals(null) && GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons[i].GetComponent<ButtonID>().buttonID.Equals(Hit.transform.GetComponent<ButtonID>().buttonID))
                    {
                        GameManager.SetAction(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons[i].GetComponent<ButtonID>().actionID);
                        if (!GameManager.actionDataBase.ActionID.Equals(0))
                            GameManager.SetPotentialDefenders(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().cardsInRange);
                        else
                            GameManager.UnsetInRange(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().cardsInRange);
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons[i].GetComponent<Image>().color = SetColor(ColorCode(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons[i].GetComponent<Image>().color));
                    }
                    else if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons[i].name.Equals(null))
                    {
                        listLength -= 1;
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons = ReworkList(i, GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().workList, GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons);
                    }
                }
            }
            else
            {
                if (getButton.Equals(Hit.transform.gameObject))
                {
                    GameManager.SetAction(getButton.GetComponent<ButtonID>().actionID);
                    getButton.GetComponent<Image>().color = SetColor(ColorCode(getButton.GetComponent<Image>().color));
                    mainDictionary.Clear();
                }
            }
        }
    }
    private Dictionary<int, GameObject> IsActive(List<GameObject> buttons, Dictionary<int, GameObject> mainDictionary)
    {
        Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>(0, null);
        foreach (GameObject button in buttons)
            if (ColorCode(button.GetComponent<Image>().color).Equals("0,700,051") && !mainDictionary.ContainsKey(1))
                dictionary.Add(1, button);
        return dictionary;
    }
    private List<GameObject> ReworkList(int i, List<GameObject> workList, List<GameObject> mainList)
    {
        for (int j = i; j < i; j++)
            if (SetBool(j, i))
                workList.Add(mainList[j]);
        return workList;
    }
    private bool SetBool(int j, int i)
    {
        bool condition;
        if (!j.Equals(i))
            condition = true;
        else
            condition = false;
        return condition;
    }
    public static Color SetColor(string colorCode) => colorCode switch
    {
        "0,06101" => GameObject.FindGameObjectWithTag("GameManager").GetComponent<ButtonAction>().selectedColor,
        "0,700,051" => GameObject.FindGameObjectWithTag("GameManager").GetComponent<ButtonAction>().normalColor,
        _ => throw new ArgumentOutOfRangeException()
    };
    public static string ColorCode(Color color)
    {
        return Math.Round(color.r, 2).ToString() + Math.Round(color.g, 2).ToString() + Math.Round(color.b, 2).ToString() + Math.Round(color.a, 2).ToString();
    }
    public static void ClearColor(List<GameObject> buttonsList)
    {
        foreach (GameObject button in buttonsList)
            if (ButtonAction.ColorCode(button.GetComponent<Image>().color).Equals("0,700,051"))
                button.GetComponent<Image>().color = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ButtonAction>().normalColor;
    }
}