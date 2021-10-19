using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultySideTrap : MonoBehaviour
{
    public GameObject[] traps;
    public bool[] directions;

    private void Start()
    {
        this.name = this.name.Replace("(Clone)", "");
        for (int i = 0; i < 4; i++)
        {
            traps[i].SetActive(directions[i]);
        }
    }
}
