using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script prevents a gameobject from being destroyed and only allows one object to be active with this script attached to it
/// </summary>
public class DoNotDestoryOnLoadScript : MonoBehaviour
{
    public DoNotDestoryOnLoadScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        DontDestroyOnLoad(instance);
    }
}
