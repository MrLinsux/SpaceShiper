using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Thicker : MonoBehaviour
{
    public GameObject A;
    public GameObject B;
    public GameObject PointA;
    public GameObject PointB;
    public GameObject disk;
    public float speed;
    public float pause;
    private Coroutine waiter;

    IEnumerator SlowDown()
    {
        var temp = speed;
        speed = 0;
        B.GetComponent<Animator>().SetBool("HaveDisk", true);
        disk.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(pause);
        B.GetComponent<Animator>().SetBool("HaveDisk", false);
        yield return new WaitUntil(() => B.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Out"));
        var tempL = A;
        A = B;
        B = tempL;
        var _temp = PointB;
        PointB = PointA;
        PointA = _temp;
        speed = temp;
        waiter = null;
        disk.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void SetPoints(Vector3 _A, Vector3 _B)
    {
        // устанавливает лаунчеры на свои места
        A.transform.position = _A; B.transform.position = _B;
    }

    private void Start()
    {
        disk.transform.position = PointA.transform.position;
        A.GetComponent<Animator>().SetBool("HaveDisk", false);
        B.GetComponent<Animator>().SetBool("HaveDisk", false);
    }


    void Update()
    {
        if (waiter == null)
        {
            if (disk.transform.position == PointB.transform.position)
            {
                waiter = StartCoroutine(SlowDown());
            }
            else
            {
                disk.transform.position = Vector3.MoveTowards(disk.transform.position, PointB.transform.position, speed / 100);
            }
        }
    }
}
