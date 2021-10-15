using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultySideTrap : MonoBehaviour
{
    public GameObject[] traps;
    public bool[] directions;

    public void SetTrap()
    {
        for (int i = 0; i < 4; i++)
        {
            traps[i].SetActive(directions[i]);
        }
    }
}
