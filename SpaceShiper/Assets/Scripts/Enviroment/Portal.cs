using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // тот самый портал-близнец
    public GameObject twinkPortal;

    public Vector3 GetTeleportPoint(Player.Direction direction)
    {
        // получаем точку появления при телепортации
        var point = twinkPortal.transform.position - Vector3.forward * (twinkPortal.transform.position.z + 1);
        switch((int)direction)
        {
            case 1:
                point += Vector3.right * twinkPortal.transform.lossyScale.x;
                break;
            case 2:
                point += Vector3.up * twinkPortal.transform.lossyScale.x;
                break;
            case 3:
                point += Vector3.left * twinkPortal.transform.lossyScale.x;
                break;
            case 4:
                point += Vector3.down * twinkPortal.transform.lossyScale.x;
                break;
        }

        return point;
    }

    private void Start()
    {
        // определяем близнеца
        if (this.transform == this.transform.parent.GetChild(0).transform)
            twinkPortal = this.transform.parent.GetChild(1).gameObject;
        else
            twinkPortal = this.transform.parent.GetChild(0).gameObject;
    }
}
