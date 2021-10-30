using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSideTrap : MonoBehaviour
{
    public Vector3 sourcePos;   // поле для сохранения в файл
    public enum Direction { Right, Up, Left, Down }

    public Direction singleDirection;

    private void Start()
    {
        sourcePos = this.transform.position;
        this.name = this.name.Replace("(Clone)", "");
        this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)singleDirection);
    }
}
