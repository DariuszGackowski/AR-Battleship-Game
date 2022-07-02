using System;
using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    [SerializeField] private Transform startPosition, endPosition, checkPoint;

    [NonSerialized] public bool shoot;
    void Update()
    {
        StartEffect();
    }
    public void StartEffect()
    {
        if (!shoot)
        {
            return;
        }

        this.transform.position = Vector3.MoveTowards(transform.position, endPosition.position, 0.1f * Time.deltaTime);

        if (this.transform.position.y > checkPoint.position.y)
        {

            this.transform.position = startPosition.position;

            shoot = false;

            this.gameObject.SetActive(false);
        }

    }
}
