using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public enum Direction { Right, Up, Left, Down }
    public enum TrapType { single, multy }

    public TrapType type;
    public Direction singleDirection;

    public GameObject[] traps;
    public bool[] directions = new bool[] { false, false, false, false };

    public void SetTrap()
    {
        if ((int)type == 0)
        {
            this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)singleDirection);
        }
        else if ((int)type == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                traps[i].SetActive(directions[i]);
            }
        }
    }

    private void Start()
    {
        Debug.Log("It's a Trap!");
    }
}