using UnityEngine;

[System.Obsolete]
public class ButtonID : MonoBehaviour
{
    public int actionID;
    public int buttonID;
    private void Start()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons.Add(this.gameObject);
        buttonID = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().buttons.IndexOf(this.gameObject);
    }
}