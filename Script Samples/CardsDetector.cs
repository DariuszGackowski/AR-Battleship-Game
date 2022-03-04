using UnityEngine;

[System.Obsolete]
public class CardsDetector : MonoBehaviour
{
    public int boxPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Card"))
            if (other.GetComponent<CardStatistics>().cardPosition.Equals(0))
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SetCardObject(other.gameObject, boxPosition, "NewCard");
            else if (!other.transform.parent.transform.parent.gameObject.GetComponent<CardStatistics>().cardPosition.Equals(boxPosition))
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SetCardObject(other.gameObject, boxPosition, "ChangePosition");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Card") && other.transform.GetChild(0).transform.GetChild(2).gameObject.activeSelf)
                GameManager.ActivateObject(other.transform.GetChild(0).transform.GetChild(2).gameObject);
    }
}