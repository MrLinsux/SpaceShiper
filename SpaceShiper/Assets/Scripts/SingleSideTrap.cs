using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSideTrap : MonoBehaviour
{
    public enum Direction { Right, Up, Left, Down }

    public Direction singleDirection;

    public void SetTrap()
    {
        this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)singleDirection);
    }
}