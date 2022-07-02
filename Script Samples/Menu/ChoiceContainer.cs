using System;
using UnityEngine;

public class ChoiceContainer : MonoBehaviour
{
    [NonSerialized] public static int decision;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
