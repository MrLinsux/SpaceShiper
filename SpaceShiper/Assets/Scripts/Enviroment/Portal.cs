using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // ��� ����� ������-�������
    public GameObject twinkPortal;

    private void Start()
    {   
        // ���������� ��������
        if (this.transform == this.transform.parent.GetChild(0).transform)
            twinkPortal = this.transform.parent.GetChild(1).gameObject;
        else
            twinkPortal = this.transform.parent.GetChild(0).gameObject;
    }
}
