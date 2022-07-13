using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Start");
    }

    int time = 0;
    void Update()
    {
        Debug.Log("Time: " + (time++));
    }
}
